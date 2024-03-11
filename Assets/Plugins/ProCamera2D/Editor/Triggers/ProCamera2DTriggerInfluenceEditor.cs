using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[CustomEditor(typeof(ProCamera2DTriggerInfluence))]
	[CanEditMultipleObjects]
    public class ProCamera2DTriggerInfluenceEditor : Editor
    {
        MonoScript _script;
        GUIContent _tooltip;

        void OnEnable()
        {
            _script = MonoScript.FromMonoBehaviour((ProCamera2DTriggerInfluence)target);
        }

        public override void OnInspectorGUI()
        {
            var proCamera2DTriggerInfluence = (ProCamera2DTriggerInfluence)target;

            if(proCamera2DTriggerInfluence.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);
            
            serializedObject.Update();

            // Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);            

            // Update interval
            _tooltip = new GUIContent("Update Interval", "Every X seconds detect collision. Smaller frequencies are more precise but also require more processing.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UpdateInterval"), _tooltip);            

            if (proCamera2DTriggerInfluence.UpdateInterval <= 0.01f)
                proCamera2DTriggerInfluence.UpdateInterval = 0.01f;

            // Trigger shape
            _tooltip = new GUIContent("Trigger Shape", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TriggerShape"), _tooltip);

            // Focus point
            _tooltip = new GUIContent("Focus Point", "If set, the focus point will be this instead of the center of the trigger");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FocusPoint"), _tooltip);

            // Influence smoothness
            _tooltip = new GUIContent("Influence Smoothness", "How smoothly should the camera move towards the focus point?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfluenceSmoothness"), _tooltip);

            if (proCamera2DTriggerInfluence.InfluenceSmoothness < 0f)
                proCamera2DTriggerInfluence.InfluenceSmoothness = 0f;

            // Exclusive influence percentage
            _tooltip = new GUIContent("Exclusive Influence Percentage", "After entering this area the camera will reach it's max zoom value");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ExclusiveInfluencePercentage"), _tooltip);

            // Trigger targets
            EditorGUILayout.BeginHorizontal();
            var tooltip = new GUIContent("Use Targets Mid Point", "If checked, the trigger will use the mid point of all your camera targets");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseTargetsMidPoint"), tooltip);

            if (!proCamera2DTriggerInfluence.UseTargetsMidPoint)
            {
                tooltip = new GUIContent("Trigger Target", "The target to use instead of the mid point of all camera targets");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TriggerTarget"), tooltip);
            }
            EditorGUILayout.EndHorizontal();

			// Mode
			_tooltip = new GUIContent("Influence Mode", "Choose what axis the influence affects");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Mode"), _tooltip);

            serializedObject.ApplyModifiedProperties();
        }
    }
}