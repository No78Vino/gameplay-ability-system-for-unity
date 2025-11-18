using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Cinemachine.Editor
{
    internal class WaveformWindow : EditorWindow
    {
        WaveformGenerator mWaveformGenerator;
        Texture2D mScreenshot;
        string mScreenshotFilename;

        // Controls how frequently (in seconds) the view will update.
        // Performance is really bad, so keep this as large as possible.
        public static float UpdateInterval = 0.5f;
        public static void SetDefaultUpdateInterval() { UpdateInterval = 0.5f; }

        //[MenuItem("Window/Waveform Monitor")]
        public static void OpenWindow()
        {
            WaveformWindow window = EditorWindow.GetWindow<WaveformWindow>(false);
            window.autoRepaintOnSceneChange = true;
            //window.position = new Rect(100, 100, 400, 400);
            window.Show(true);
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Waveform", CinemachineSettings.CinemachineLogoTexture);
            mWaveformGenerator = new WaveformGenerator();

            mScreenshotFilename = Path.GetFullPath(FileUtil.GetUniqueTempPathInProject() + ".png");
            ScreenCapture.CaptureScreenshot(mScreenshotFilename);
            EditorApplication.update += UpdateScreenshot;
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateScreenshot;
            if (!string.IsNullOrEmpty(mScreenshotFilename) && File.Exists(mScreenshotFilename))
                File.Delete(mScreenshotFilename);
            mScreenshotFilename = null;
            mWaveformGenerator.DestroyBuffers();
            if (mScreenshot != null)
                DestroyImmediate(mScreenshot);
            mScreenshot = null;
        }

        private void OnGUI()
        {
            Rect rect = EditorGUILayout.GetControlRect(true);
            EditorGUIUtility.labelWidth /= 2;
            EditorGUI.BeginChangeCheck();
            mWaveformGenerator.m_Exposure = EditorGUI.Slider(
                rect, "Exposure", mWaveformGenerator.m_Exposure, 0.01f, 2);
            if (EditorGUI.EndChangeCheck())
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            EditorGUIUtility.labelWidth *= 2;
            rect.y += rect.height;
            rect.height = position.height - rect.height;
            var tex = mWaveformGenerator.Result;
            if (tex != null)
                GUI.DrawTexture(rect, tex);
        }

        float mLastUpdateTime = 0;
        private void UpdateScreenshot()
        {
            // Don't do this too often
            float now = Time.time;
            if (mScreenshot != null && now - mLastUpdateTime < UpdateInterval)
                return;

            mLastUpdateTime = now;
            if (!string.IsNullOrEmpty(mScreenshotFilename) && File.Exists(mScreenshotFilename))
            {
                byte[] fileData = File.ReadAllBytes(mScreenshotFilename);
                if (mScreenshot == null)
                    mScreenshot = new Texture2D(2, 2);
                mScreenshot.LoadImage(fileData); // this will auto-resize the texture dimensions.
                mWaveformGenerator.RenderWaveform(mScreenshot);

                // Capture the next one
                ScreenCapture.CaptureScreenshot(mScreenshotFilename);
            }
        }

        class WaveformGenerator
        {
            public float m_Exposure = 0.2f;

            RenderTexture mOutput;
            ComputeBuffer mData;

            int mThreadGroupSize;
            int mThreadGroupSizeX;
            int mThreadGroupSizeY;

            ComputeShader mWaveformCompute;
            MaterialPropertyBlock mWaveformProperties;
            Material mWaveformMaterial;
            CommandBuffer mCmd;

            static Mesh sFullscreenTriangle;
            static Mesh FullscreenTriangle
            {
                get
                {
                    if (sFullscreenTriangle == null)
                    {
                        sFullscreenTriangle = new Mesh { name = "Fullscreen Triangle" };
                        sFullscreenTriangle.SetVertices(new List<Vector3>
                        {
                            new Vector3(-1f, -1f, 0f),
                            new Vector3(-1f,  3f, 0f),
                            new Vector3( 3f, -1f, 0f)
                        });
                        sFullscreenTriangle.SetIndices(
                            new [] { 0, 1, 2 }, MeshTopology.Triangles, 0, false);
                        sFullscreenTriangle.UploadMeshData(false);
                    }
                    return sFullscreenTriangle;
                }
            }

            public WaveformGenerator()
            {
                mWaveformCompute = AssetDatabase.LoadAssetAtPath<ComputeShader>(
                        ScriptableObjectUtility.CinemachineRealativeInstallPath
                            + "/Editor/EditorResources/CMWaveform.compute");
                mWaveformProperties = new MaterialPropertyBlock();
                mWaveformMaterial = new Material(AssetDatabase.LoadAssetAtPath<Shader>(
                    ScriptableObjectUtility.CinemachineRealativeInstallPath
                        + "/Editor/EditorResources/CMWaveform.shader"))
                {
                    name = "CMWaveformMaterial",
                    hideFlags = HideFlags.DontSave
                };
                mCmd = new CommandBuffer();
            }

            void CreateBuffers(int width, int height)
            {
                if (mOutput == null || !mOutput.IsCreated()
                    || mOutput.width != width || mOutput.height != height)
                {
                    DestroyImmediate(mOutput);
                    mOutput = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
                    {
                        anisoLevel = 0,
                        filterMode = FilterMode.Bilinear,
                        wrapMode = TextureWrapMode.Clamp,
                        useMipMap = false
                    };
                    mOutput.Create();
                }

                int count = Mathf.CeilToInt(width / (float)mThreadGroupSizeX) * mThreadGroupSizeX * height;
                if (mData == null)
                    mData = new ComputeBuffer(count, sizeof(uint) << 2);
                else if (mData.count < count)
                {
                    mData.Release();
                    mData = new ComputeBuffer(count, sizeof(uint) << 2);
                }
            }

            public void DestroyBuffers()
            {
                if (mData != null)
                    mData.Release();
                mData = null;
                DestroyImmediate(mOutput);
                mOutput = null;
            }

            public RenderTexture Result { get { return mOutput; } }

            public void RenderWaveform(Texture2D source)
            {
                if (mWaveformMaterial == null)
                    return;

                int width = source.width;
                int height = source.height;
                int histogramResolution = 256;

                mThreadGroupSize = 256;
                mThreadGroupSizeX = 16;
                mThreadGroupSizeY = 16;
                CreateBuffers(width, histogramResolution);

                mCmd.Clear();
                mCmd.BeginSample("CMWaveform");

                var parameters = new Vector4(
                    width, height,
                    QualitySettings.activeColorSpace == ColorSpace.Linear ? 1 : 0,
                    histogramResolution);

                // Clear the buffer on every frame
                int kernel = mWaveformCompute.FindKernel("KCMWaveformClear");
                mCmd.SetComputeBufferParam(mWaveformCompute, kernel, "_WaveformBuffer", mData);
                mCmd.SetComputeVectorParam(mWaveformCompute, "_Params", parameters);
                mCmd.DispatchCompute(mWaveformCompute, kernel,
                    Mathf.CeilToInt(width / (float)mThreadGroupSizeX),
                    Mathf.CeilToInt(histogramResolution / (float)mThreadGroupSizeY), 1);

                // Gather all pixels and fill in our waveform
                kernel = mWaveformCompute.FindKernel("KCMWaveformGather");
                mCmd.SetComputeBufferParam(mWaveformCompute, kernel, "_WaveformBuffer", mData);
                mCmd.SetComputeTextureParam(mWaveformCompute, kernel, "_Source", source);
                mCmd.SetComputeVectorParam(mWaveformCompute, "_Params", parameters);
                mCmd.DispatchCompute(mWaveformCompute, kernel, width,
                    Mathf.CeilToInt(height / (float)mThreadGroupSize), 1);

                // Generate the waveform texture
                float exposure = Mathf.Max(0f, m_Exposure);
                exposure *= (float)histogramResolution / height;
                mWaveformProperties.SetVector(Shader.PropertyToID("_Params"),
                    new Vector4(width, histogramResolution, exposure, 0f));
                mWaveformProperties.SetBuffer(Shader.PropertyToID("_WaveformBuffer"), mData);
                mCmd.SetRenderTarget(mOutput);
                mCmd.DrawMesh(
                    FullscreenTriangle, Matrix4x4.identity,
                    mWaveformMaterial, 0, 0, mWaveformProperties);
                mCmd.EndSample("CMWaveform");

                Graphics.ExecuteCommandBuffer(mCmd);
            }
        }
    }
}
