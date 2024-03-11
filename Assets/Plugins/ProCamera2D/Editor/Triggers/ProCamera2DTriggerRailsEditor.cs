using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DTriggerRails))]
    [CanEditMultipleObjects]
    public class ProCamera2DTriggerRailsEditor : Editor
    {
        MonoScript _script;
        GUIContent _tooltip;

        void OnEnable()
        {
            _script = MonoScript.FromMonoBehaviour((ProCamera2DTriggerRails)target);

            var proCamera2DTriggerRails = (ProCamera2DTriggerRails)target;

            if(proCamera2DTriggerRails.ProCamera2D != null && proCamera2DTriggerRails.ProCamera2DRails == null)
                proCamera2DTriggerRails.ProCamera2DRails = proCamera2DTriggerRails.ProCamera2D.GetComponentInChildren<ProCamera2DRails>();
        }

        public override void OnInspectorGUI()
        {
            var proCamera2DTriggerRails = (ProCamera2DTriggerRails)target;

            if(proCamera2DTriggerRails.ProCamera2D == null)
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

            if (proCamera2DTriggerRails.UpdateInterval <= 0.01f)
                proCamera2DTriggerRails.UpdateInterval = 0.01f;

            // Trigger shape
            _tooltip = new GUIContent("Trigger Shape", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TriggerShape"), _tooltip);

            // Trigger targets
            EditorGUILayout.BeginHorizontal();
            _tooltip = new GUIContent("Use Targets Mid Point", "If checked, the trigger will use the mid point of all your camera targets");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseTargetsMidPoint"), _tooltip);

            if (!proCamera2DTriggerRails.UseTargetsMidPoint)
            {
                _tooltip = new GUIContent("Trigger Target", "The target to use instead of the mid point of all camera targets");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TriggerTarget"), _tooltip);
            }
            EditorGUILayout.EndHorizontal();

            // Mode
            _tooltip = new GUIContent("Mode", "Choose if you want this trigger to enable or disable the rails.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Mode"), _tooltip);

            // Transition Duration
            _tooltip = new GUIContent("Transition Duration", "The time it should take to transition from/to rails.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TransitionDuration"), _tooltip);


            serializedObject.ApplyModifiedProperties();
        }
    }
}