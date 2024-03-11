using UnityEngine;
using UnityEditor;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ShakePreset))]
    public class ShakePresetEditor : Editor
    {
        GUIContent _tooltip;

        ShakePreset _preset;

        void OnEnable()
        {
            _preset = (ShakePreset)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Strength
            _tooltip = new GUIContent("Strength", "The shake strength on each axis");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Strength"), _tooltip);

            // Duration
            _tooltip = new GUIContent("Duration", "The duration of the shake");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Duration"), _tooltip);

            // Vibrato
            _tooltip = new GUIContent("Vibrato", "Indicates how much will the shake vibrate");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Vibrato"), _tooltip);

            // Smoothness
            _tooltip = new GUIContent("Smoothness", "Indicates how smooth the shake will be");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Smoothness"), _tooltip);

            // Randomness
            _tooltip = new GUIContent("Randomness", "Indicates how random the shake will be");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Randomness"), _tooltip);

            // Random initial direction
            EditorGUILayout.BeginHorizontal();
            _tooltip = new GUIContent("Use Random Initial Angle", "If enabled, the initial shaking angle will be random");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseRandomInitialAngle"), _tooltip);

            if (!_preset.UseRandomInitialAngle)
            {
                _tooltip = new GUIContent("Initial Angle", "");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("InitialAngle"), _tooltip);
            }
            EditorGUILayout.EndHorizontal();

            // Rotation
            _tooltip = new GUIContent("Rotation", "The maximum rotation the camera will reach");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Rotation"), _tooltip);

            // Ignore time scale
            _tooltip = new GUIContent("Ignore TimeScale", "If enabled, the shake will occur even if the timeScale is 0");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IgnoreTimeScale"), _tooltip);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Shake test buttons
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Shake!"))
            {
                if (ProCamera2DShake.Exists)
                    ProCamera2DShake.Instance.Shake(_preset);
            }

            if (GUILayout.Button("Stop!"))
            {
                if (ProCamera2DShake.Exists)
                    ProCamera2DShake.Instance.StopShaking();
            }
            GUI.enabled = true;

            if (GUILayout.Button("Go to ProCamera2D"))
            {
                Selection.activeGameObject = ProCamera2D.Instance.gameObject;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}