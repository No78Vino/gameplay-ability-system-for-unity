using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DLimitSpeed))]
    public class ProCamera2DLimitSpeedEditor : Editor
    {
        GUIContent _tooltip;

        MonoScript _script;

        void OnEnable()
        {
            if (target == null)
                return;
            
            _script = MonoScript.FromMonoBehaviour((ProCamera2DLimitSpeed)target);
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            
            var proCamera2DLimitSpeed = (ProCamera2DLimitSpeed)target;

            serializedObject.Update();

            // Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);

            if(proCamera2DLimitSpeed.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);

            // Max speed horizontal
            EditorGUILayout.BeginHorizontal();

            _tooltip = new GUIContent("Limit Horizontal Speed", "Limit how fast the camera moves per second on the horizontal axis");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LimitHorizontalSpeed"), _tooltip);

            if (proCamera2DLimitSpeed.LimitHorizontalSpeed)
            {
                _tooltip = new GUIContent(" ", "Limit how fast the camera moves per second on the horizontal axis");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxHorizontalSpeed"), _tooltip);
            }

            EditorGUILayout.EndHorizontal();

            // Max speed vertical
            EditorGUILayout.BeginHorizontal();

            _tooltip = new GUIContent("Limit Vertical Speed", "Limit how fast the camera moves per second on the vertical axis");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LimitVerticalSpeed"), _tooltip);

            if (proCamera2DLimitSpeed.LimitVerticalSpeed)
            {
                _tooltip = new GUIContent(" ", "Limit how fast the camera moves per second on the vertical axis");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxVerticalSpeed"), _tooltip);
            }

            EditorGUILayout.EndHorizontal();

            // Limit values
            if (proCamera2DLimitSpeed.MaxHorizontalSpeed <= 0f)
                proCamera2DLimitSpeed.MaxHorizontalSpeed = .01f;

            if (proCamera2DLimitSpeed.MaxVerticalSpeed <= 0f)
                proCamera2DLimitSpeed.MaxVerticalSpeed = .01f;

            // Show current speed
            EditorGUILayout.Space();
            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Current Speed");
            GUILayout.Label("H: " + Mathf.Abs(proCamera2DLimitSpeed.CurrentSpeedHorizontal).ToString("F") + "    V: " + Mathf.Abs(proCamera2DLimitSpeed.CurrentSpeedVertical).ToString("F"));
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
        }
    }
}