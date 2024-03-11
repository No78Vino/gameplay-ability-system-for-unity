using UnityEngine;
using UnityEditor;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ConstantShakePreset))]
    public class ConstantShakePresetEditor : Editor
    {
        GUIContent _tooltip;

        ConstantShakePreset _preset;

        void OnEnable()
        {
            _preset = (ConstantShakePreset)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Intensity
            _tooltip = new GUIContent("Intensity", "How fast the camera moves towards the amplitude");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Intensity"), _tooltip);

            // Layers
            _tooltip = new GUIContent("Layers", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Layers"), _tooltip, true);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Shake test buttons
            GUI.enabled = Application.isPlaying && ProCamera2DShake.Exists;
            if (GUILayout.Button("Constant Shake!"))
            {
                ProCamera2DShake.Instance.ConstantShake(_preset);
            }

            if (GUILayout.Button("Stop!"))
            {
                ProCamera2DShake.Instance.StopConstantShaking();
            }
            GUI.enabled = true;

            if (GUILayout.Button("Go to ProCamera2D"))
            {
                if (ProCamera2D.Instance != null)
                {
                    Selection.activeGameObject = ProCamera2D.Instance.gameObject;
                }
            }

            if (_preset.Intensity < .01f)
                _preset.Intensity = .01f;

            serializedObject.ApplyModifiedProperties();
        }
    }
}