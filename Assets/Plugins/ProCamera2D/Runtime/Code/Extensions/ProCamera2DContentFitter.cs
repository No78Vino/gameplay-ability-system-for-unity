using System;
using System.Collections;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public enum ContentFitterMode
    {
        AspectRatio,
        FixedWidth,
        FixedHeight
    }

    [HelpURLAttribute("http://www.procamera2d.com/user-guide/extension-content-fitter/")]
    public class ProCamera2DContentFitter : BasePC2D, ISizeOverrider
    {
        public static string ExtensionName = "Content Fitter";

        public ContentFitterMode ContentFitterMode
        {
            set
            {
                _contentFitterMode = value;
                ProCamera2D.GameCamera.ResetProjectionMatrix();
                
                if(_contentFitterMode == ContentFitterMode.AspectRatio)
                    TargetWidth = TargetHeight * TargetAspectRatio;
            }
            get { return _contentFitterMode; }
        }

        [SerializeField] private ContentFitterMode _contentFitterMode;

        public bool UseLetterOrPillarboxing
        {
            set
            {
                _useLetterOrPillarboxing = value;
                ToggleLetterPillarboxing(value);
            }
            get { return _useLetterOrPillarboxing; }
        }

        [SerializeField] private bool _useLetterOrPillarboxing;

        private static float ScreenAspectRatio
        {
            get { return Screen.width / (float) Screen.height; }
        }

        public float TargetHeight
        {
            get { return _targetHeight; }
            set
            {
                _targetHeight = value;
                _targetWidth = ContentFitterMode == ContentFitterMode.AspectRatio ? value * TargetAspectRatio : value * ScreenAspectRatio;
            }
        }
        [SerializeField]
        private float _targetHeight = 5.625f;
        

        public float TargetWidth
        {
            get { return _targetWidth; }
            set
            {
                _targetWidth = value;
                _targetHeight = ContentFitterMode == ContentFitterMode.AspectRatio ? value / TargetAspectRatio : value / ScreenAspectRatio;
            }
        }
        [SerializeField]
        private float _targetWidth = 10;

        public float TargetAspectRatio
        {
            get { return _targetAspectRatio; }
            set
            {
                _targetAspectRatio = value;
                _targetWidth = _targetHeight * value;
            }
        }
        [Range(.1f, 3f)]
        [SerializeField]
        private float _targetAspectRatio = 16 / 9f;

        [Range(-1, 1)] public float VerticalAlignment;

        [Range(-1, 1)] public float HorizontalAlignment;

        private float
            _prevTargetHeight,
            _prevTargetWidth,
            _prevTargetAspectRatio,
            _prevAspectRatio,
            _prevVerticalAlignment,
            _prevHorizontalAlignment;

        private bool _prevUseLetterOrPillarboxing;

        private Camera _letterPillarboxingCamera;

        protected override void Awake()
        {
            base.Awake();

            ProCamera2D.AddSizeOverrider(this);
        }

        private IEnumerator Start()
        {
            if (UseLetterOrPillarboxing)
                CreateLetterPillarboxingCamera();

            yield return null;

            if (ContentFitterMode == ContentFitterMode.AspectRatio)
                UpdateCameraAlignment(
                    ProCamera2D.GameCamera,
                    TargetHeight * .5f > TargetWidth * .5f / ProCamera2D.GameCamera.aspect,
                    TargetAspectRatio,
                    HorizontalAlignment,
                    VerticalAlignment);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(ProCamera2D)
                ProCamera2D.RemoveSizeOverrider(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            ProCamera2D.GameCamera.ResetProjectionMatrix();
        }

#if UNITY_EDITOR
        protected override void DrawGizmosSelected()
        {
            base.DrawGizmosSelected();
            
            var fillColor = EditorPrefsX.GetColor(PrefsData.FitterFillColorKey, PrefsData.FitterFillColorValue);
            var lineColor = EditorPrefsX.GetColor(PrefsData.FitterLineColorKey, PrefsData.FitterFillColorValue);

            Gizmos.color = lineColor;
                
            var camSize =
                Utils.GetScreenSizeInWorldCoords(ProCamera2D.GameCamera, Mathf.Abs(Vector3D(transform.localPosition)));
            var camPos = VectorHVD(Vector3H(ProCamera2D.transform.position), Vector3V(ProCamera2D.transform.position),
                0);

            var rectCorners = DrawGizmoRectangle(
                Vector3H(ProCamera2D.transform.position),
                Vector3V(ProCamera2D.transform.position),
                ContentFitterMode != ContentFitterMode.FixedHeight ? TargetWidth : camSize.x,
                ContentFitterMode != ContentFitterMode.FixedWidth ? TargetHeight : camSize.y,
                fillColor,
                lineColor);

            if (_contentFitterMode == ContentFitterMode.AspectRatio)
            {
                if (TargetHeight * .5f > TargetWidth * .5f / ProCamera2D.GameCamera.aspect)
                {
                    DrawGizmoRectangle(
                        Vector3H(ProCamera2D.transform.position) - HorizontalAlignment * (TargetHeight * ProCamera2D.GameCamera.aspect - TargetWidth) / 2f,
                        Vector3V(ProCamera2D.transform.position),
                        TargetHeight * ProCamera2D.GameCamera.aspect,
                        TargetHeight,
                        fillColor,
                        lineColor);
                }
                else
                {
                    DrawGizmoRectangle(
                        Vector3H(ProCamera2D.transform.position),
                        Vector3V(ProCamera2D.transform.position) - VerticalAlignment * (TargetWidth / ProCamera2D.GameCamera.aspect - TargetHeight) / 2f,
                        TargetWidth,
                        TargetWidth / ProCamera2D.GameCamera.aspect,
                        fillColor,
                        lineColor);
                }

                Utils.DrawArrowForGizmo(camPos, camPos - rectCorners[0], camSize.y * .03f);
                Utils.DrawArrowForGizmo(camPos, camPos - rectCorners[2], camSize.y * .03f);
            }
            else if (_contentFitterMode == ContentFitterMode.FixedWidth)
            {
                DrawGizmoRectangle(
                    Vector3H(ProCamera2D.transform.position),
                    Vector3V(ProCamera2D.transform.position),
                    TargetWidth,
                    TargetWidth / ProCamera2D.GameCamera.aspect,
                    fillColor,
                    lineColor);

                Utils.DrawArrowForGizmo(camPos, ProCamera2D.transform.right * TargetWidth * .5f, camSize.y * .03f);
                Utils.DrawArrowForGizmo(camPos, -ProCamera2D.transform.right * TargetWidth * .5f, camSize.y * .03f);
            }
            else
            {
                DrawGizmoRectangle(
                    Vector3H(ProCamera2D.transform.position),
                    Vector3V(ProCamera2D.transform.position),
                    TargetHeight * ProCamera2D.GameCamera.aspect,
                    TargetHeight,
                    fillColor,
                    lineColor);

                Utils.DrawArrowForGizmo(camPos, ProCamera2D.transform.up * TargetHeight * .5f, camSize.y * .03f);
                Utils.DrawArrowForGizmo(camPos, -ProCamera2D.transform.up * TargetHeight * .5f, camSize.y * .03f);
            }
        }
        #endif

        #region ISizeOverrider implementation

        public float OverrideSize(float deltaTime, float originalSize)
        {
            return !enabled ? originalSize : GetSize(ContentFitterMode);
        }

        public int SOOrder
        {
            get { return _soOrder; }
            set { _soOrder = value; }
        }

        private int _soOrder = 5000;

        #endregion

        private float GetSize(ContentFitterMode mode)
        {
            switch (mode)
            {
                case ContentFitterMode.FixedHeight:
                    return TargetHeight * .5f;
                case ContentFitterMode.FixedWidth:
                    return TargetWidth * .5f / ProCamera2D.GameCamera.aspect;
                case ContentFitterMode.AspectRatio:
                    if (_prevTargetWidth != TargetWidth ||
                        _prevTargetHeight != TargetHeight ||
                        _prevTargetAspectRatio != TargetAspectRatio ||
                        _prevAspectRatio != ProCamera2D.GameCamera.aspect ||
                        _prevVerticalAlignment != VerticalAlignment ||
                        _prevHorizontalAlignment != HorizontalAlignment ||
                        _prevUseLetterOrPillarboxing != _useLetterOrPillarboxing)
                    {
                        StartCoroutine(UpdateFixedAspectRatio());
                    }

                    _prevTargetWidth = TargetWidth;
                    _prevTargetHeight = TargetHeight;
                    _prevTargetAspectRatio = TargetAspectRatio;
                    _prevAspectRatio = ProCamera2D.GameCamera.aspect;
                    _prevVerticalAlignment = VerticalAlignment;
                    _prevHorizontalAlignment = HorizontalAlignment;
                    _prevUseLetterOrPillarboxing = _useLetterOrPillarboxing;

                    return Mathf.Max(TargetHeight * .5f, TargetWidth * .5f / ProCamera2D.GameCamera.aspect);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        IEnumerator UpdateFixedAspectRatio()
        {
            var isPillarbox = TargetHeight * .5f > TargetWidth * .5f / ScreenAspectRatio;
                        
            if(_prevUseLetterOrPillarboxing != _useLetterOrPillarboxing)
                ToggleLetterPillarboxing(_useLetterOrPillarboxing);

            if (UseLetterOrPillarboxing)
            {
                UpdateLetterPillarbox(
                    ProCamera2D.GameCamera, 
                    isPillarbox, 
                    TargetAspectRatio,
                    HorizontalAlignment,
                    VerticalAlignment);
            }
            
            yield return new WaitForEndOfFrame();
            
            UpdateCameraAlignment(
                ProCamera2D.GameCamera,
                isPillarbox,
                TargetAspectRatio,
                HorizontalAlignment,
                VerticalAlignment);
        }

        private static void UpdateCameraAlignment(
            Camera cam,
            bool isPillarbox,
            float targetAspectRatio,
            float horizontalAlignment,
            float verticalAlignment)
        {
            cam.ResetProjectionMatrix();

            var horizontalOffset = isPillarbox
                ? (-.5f + (targetAspectRatio / cam.aspect) * 0.5f) * horizontalAlignment
                : 0;

            var verticalOffset = !isPillarbox
                ? (-.5f + (cam.aspect / targetAspectRatio) * 0.5f) * verticalAlignment
                : 0;

            cam.projectionMatrix = GetScissorRect(
                new Rect(
                    horizontalOffset,
                    verticalOffset,
                    1, 1), cam.projectionMatrix);
        }
        
        private static Matrix4x4 GetScissorRect(Rect targetScissor, Matrix4x4 camProjectionMatrix)
        {
            var m2 = Matrix4x4.TRS(
                new Vector3((1 / targetScissor.width - 1), (1 / targetScissor.height - 1), 0),
                Quaternion.identity,
                new Vector3(1 / targetScissor.width, 1 / targetScissor.height, 1));
            var m3 = Matrix4x4.TRS(
                new Vector3(-targetScissor.x * 2 / targetScissor.width, -targetScissor.y * 2 / targetScissor.height, 0),
                Quaternion.identity,
                Vector3.one);

            return m3 * m2 * camProjectionMatrix;
        }

        private static void UpdateLetterPillarbox(
            Camera cam, 
            bool isPillarbox, 
            float targetAspectRatio, 
            float horizontalAlignment, 
            float verticalAlignment)
        {
            if (isPillarbox)
            {
                var inset = 1.0f - targetAspectRatio / (Screen.width / (float) Screen.height);
                cam.rect = new Rect((inset / 2f) + (inset / 2f) * horizontalAlignment, 0.0f, 1.0f - inset, 1.0f);
            }
            else
            {
                var inset = 1.0f - (Screen.width / (float) Screen.height) / targetAspectRatio;
                cam.rect = new Rect(0.0f, (inset / 2f) + (inset / 2f) * verticalAlignment, 1.0f, 1.0f - inset);
            }
        }

        private void ToggleLetterPillarboxing(bool value)
        {
            if (value && _letterPillarboxingCamera == null)
                CreateLetterPillarboxingCamera();

            if (value)
            {
                _letterPillarboxingCamera.gameObject.SetActive(true);
                UpdateLetterPillarbox(
                    ProCamera2D.GameCamera, 
                    TargetHeight * .5f > TargetWidth * .5f / ScreenAspectRatio, 
                    TargetAspectRatio,
                    HorizontalAlignment,
                    VerticalAlignment);
            }
            else
            {
                if (_letterPillarboxingCamera != null)
                    _letterPillarboxingCamera.gameObject.SetActive(false);

                ProCamera2D.GameCamera.rect = new Rect(0, 0, 1, 1);
                
                UpdateCameraAlignment(
                    ProCamera2D.GameCamera,
                    TargetHeight * .5f > TargetWidth * .5f / ScreenAspectRatio,
                    TargetAspectRatio,
                    HorizontalAlignment,
                    VerticalAlignment);
            }
        }

        private void CreateLetterPillarboxingCamera()
        {
            _letterPillarboxingCamera = new GameObject("PC2DBackgroundCamera", typeof(Camera)).GetComponent<Camera>();
            _letterPillarboxingCamera.depth = int.MinValue;
            _letterPillarboxingCamera.clearFlags = CameraClearFlags.SolidColor;
            _letterPillarboxingCamera.backgroundColor = Color.black;
            _letterPillarboxingCamera.cullingMask = 0;
            _letterPillarboxingCamera.transform.position = new Vector3(10000, 10000, 10000);
            _letterPillarboxingCamera.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        private Vector3[] DrawGizmoRectangle(float x, float y, float width, float height, Color fillColor,
            Color borderColor)
        {
            var rect = new Rect(x, y, width, height);
            rect.x -= rect.width / 2f;
            rect.y -= rect.height / 2f;
            Vector3[] rectangleCorners =
            {
                VectorHVD(rect.position.x, rect.position.y, 0), // Bottom Left
                VectorHVD(rect.position.x + rect.width, rect.position.y, 0), // Bottom Right
                VectorHVD(rect.position.x + rect.width, rect.position.y + rect.height, 0), // Top Right
                VectorHVD(rect.position.x, rect.position.y + rect.height, 0) // Top Left
            };
            
            #if UNITY_EDITOR
            UnityEditor.Handles.DrawSolidRectangleWithOutline(rectangleCorners, fillColor, borderColor);
            #endif
            
            return rectangleCorners;
        }
    }
}