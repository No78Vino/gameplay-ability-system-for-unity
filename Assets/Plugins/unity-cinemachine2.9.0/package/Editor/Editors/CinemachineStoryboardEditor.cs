#if !UNITY_2019_1_OR_NEWER
#define CINEMACHINE_UGUI
#endif

#if CINEMACHINE_UGUI
using UnityEngine;
using UnityEditor;

namespace Cinemachine.Editor
{
    [InitializeOnLoad]
    static class CinemachineStoryboardMute
    {
        static CinemachineStoryboardMute()
        {
            CinemachineStoryboard.s_StoryboardGlobalMute = Enabled;
        }

        public static string kEnabledKey = "StoryboardMute_Enabled";
        public static bool Enabled
        {
            get { return EditorPrefs.GetBool(kEnabledKey, false); }
            set
            {
                if (value != Enabled)
                {
                    EditorPrefs.SetBool(kEnabledKey, value);
                    CinemachineStoryboard.s_StoryboardGlobalMute = value;
                    InspectorUtility.RepaintGameView();
                }
            }
        }
    }

    [CustomEditor(typeof(CinemachineStoryboard))]
    [CanEditMultipleObjects]
    internal sealed class CinemachineStoryboardEditor : BaseEditor<CinemachineStoryboard>
    {
        public void OnDisable()
        {
            WaveformWindow.SetDefaultUpdateInterval();
        }

        const float FastWaveformUpdateInterval = 0.1f;
        float mLastSplitScreenEventTime = 0;
        static bool sAdvancedFoldout;
        
        public override void OnInspectorGUI()
        {
            float now = Time.realtimeSinceStartup;
            if (now - mLastSplitScreenEventTime > FastWaveformUpdateInterval * 5)
                WaveformWindow.SetDefaultUpdateInterval();

            BeginInspector();
            CinemachineStoryboardMute.Enabled
                = EditorGUILayout.Toggle(
                    new GUIContent(
                        "Storyboard Global Mute",
                        "If checked, all storyboards are globally muted."),
                    CinemachineStoryboardMute.Enabled);

            Rect rect = EditorGUILayout.GetControlRect(true);
            EditorGUI.BeginChangeCheck();
            {
                float width = rect.width;
                rect.width = EditorGUIUtility.labelWidth + rect.height;
                EditorGUI.PropertyField(rect, FindProperty(x => x.m_ShowImage));

                rect.x += rect.width; rect.width = width - rect.width;
                EditorGUI.PropertyField(rect, FindProperty(x => x.m_Image), GUIContent.none);

                EditorGUILayout.PropertyField(FindProperty(x => x.m_Aspect));
                EditorGUILayout.PropertyField(FindProperty(x => x.m_Alpha));
                EditorGUILayout.PropertyField(FindProperty(x => x.m_Center));
                EditorGUILayout.PropertyField(FindProperty(x => x.m_Rotation));

                rect = EditorGUILayout.GetControlRect(true);
                EditorGUI.LabelField(rect, "Scale");
                rect.x += EditorGUIUtility.labelWidth; rect.width -= EditorGUIUtility.labelWidth;
                rect.width /= 3;
                serializedObject.SetIsDifferentCacheDirty(); // prop.hasMultipleDifferentValues always results in false if the SO isn't refreshed here
                var prop = FindProperty(x => x.m_SyncScale);
                var syncHasDifferentValues = prop.hasMultipleDifferentValues;
                GUIContent syncLabel = new GUIContent("Sync", prop.tooltip);
                EditorGUI.showMixedValue = syncHasDifferentValues;
                prop.boolValue = EditorGUI.ToggleLeft(rect, syncLabel, prop.boolValue);
                EditorGUI.showMixedValue = false;
                rect.x += rect.width;
                if (prop.boolValue || targets.Length > 1 && syncHasDifferentValues)
                {
                    prop = FindProperty(x => x.m_Scale);
                    float[] values = new float[1] { prop.vector2Value.x };
                    EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
                    EditorGUI.MultiFloatField(rect, new GUIContent[1] { new GUIContent("X") }, values);
                    EditorGUI.showMixedValue = false;
                    prop.vector2Value = new Vector2(values[0], values[0]);
                }
                else
                {
                    rect.width *= 2;
                    prop = FindProperty(x => x.m_Scale);
                    EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
                    EditorGUI.PropertyField(rect, prop, GUIContent.none);
                    EditorGUI.showMixedValue = false;
                }
                EditorGUILayout.PropertyField(FindProperty(x => x.m_MuteCamera));
            }
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(FindProperty(x => x.m_SplitView));
            if (EditorGUI.EndChangeCheck())
            {
                mLastSplitScreenEventTime = now;
                WaveformWindow.UpdateInterval = FastWaveformUpdateInterval;
                serializedObject.ApplyModifiedProperties();
            }
            rect = EditorGUILayout.GetControlRect(true);
            GUI.Label(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height),
                "Waveform Monitor");
            rect.width -= EditorGUIUtility.labelWidth; rect.width /= 2;
            rect.x += EditorGUIUtility.labelWidth;
            if (GUI.Button(rect, "Open"))
                WaveformWindow.OpenWindow();

            EditorGUILayout.Space();
            sAdvancedFoldout = EditorGUILayout.Foldout(sAdvancedFoldout, "Advanced");
            if (sAdvancedFoldout)
            {
                ++EditorGUI.indentLevel;
                
                EditorGUI.BeginChangeCheck();
                var renderModeProperty = FindProperty(x => x.m_RenderMode);
                EditorGUILayout.PropertyField(renderModeProperty);
                EditorGUILayout.PropertyField(FindProperty(x => x.m_SortingOrder));
                if (renderModeProperty.enumValueIndex == (int) RenderMode.ScreenSpaceCamera)
                {
                    EditorGUILayout.PropertyField(FindProperty(x => x.m_PlaneDistance));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }
                
                --EditorGUI.indentLevel;
            }
        }
    }
}
#endif
