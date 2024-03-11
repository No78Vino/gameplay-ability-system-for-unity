using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[CustomEditor(typeof(ProCamera2DTriggerZoom))]
	[CanEditMultipleObjects]
    public class ProCamera2DTriggerZoomEditor : Editor
    {
        MonoScript _script;
        GUIContent _tooltip;

        void OnEnable()
        {
            _script = MonoScript.FromMonoBehaviour((ProCamera2DTriggerZoom)target);
        }

        public override void OnInspectorGUI()
        {
            var proCamera2DTriggerZoom = (ProCamera2DTriggerZoom)target;

            if(proCamera2DTriggerZoom.ProCamera2D == null)
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

            if (proCamera2DTriggerZoom.UpdateInterval <= 0.01f)
                proCamera2DTriggerZoom.UpdateInterval = 0.01f;

            // Trigger shape
            _tooltip = new GUIContent("Trigger Shape", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TriggerShape"), _tooltip);

            // Set size as multiplier
            _tooltip = new GUIContent("Set Size As Multiplier", "If checked, you set target size of the camera as a zoom multiplier of the initial camera size");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SetSizeAsMultiplier"), _tooltip);

            // Target zoom
            if(proCamera2DTriggerZoom.SetSizeAsMultiplier)
                _tooltip = new GUIContent("Zoom Multiplier", "The amount of zoom the camera should do when entering this trigger");
            else if(proCamera2DTriggerZoom.ProCamera2D.GameCamera.orthographic)
                _tooltip = new GUIContent("Target Ortho Size", "The target size of the camera when entering this trigger");
            else 
                _tooltip = new GUIContent("Target FOV Size", "The target size of the camera when entering this trigger");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("TargetZoom"), _tooltip);

            if (proCamera2DTriggerZoom.TargetZoom <= 0.01f)
                proCamera2DTriggerZoom.TargetZoom = 0.01f;

            // Zoom smoothness
            _tooltip = new GUIContent("Zoom Smoothness", "How smooth should the zoom be?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ZoomSmoothness"), _tooltip);
            
            if (proCamera2DTriggerZoom.ZoomSmoothness < 0f)
                proCamera2DTriggerZoom.ZoomSmoothness = 0f;

            // Exclusive influence percentage
            _tooltip = new GUIContent("Exclusive Influence Percentage", "After entering this area the camera will reach it's max zoom value");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ExclusiveInfluencePercentage"), _tooltip);

            // Reset size on exit
            EditorGUILayout.BeginHorizontal();
            _tooltip = new GUIContent("Reset Size On Leave", "As the target leaves the trigger area the camera will reset its size to the start value");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ResetSizeOnExit"), _tooltip);

            if (proCamera2DTriggerZoom.ResetSizeOnExit)
            {
                // Reset size smoothness
                _tooltip = new GUIContent("Smoothness", "How smoothly should the camera resit its size?");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ResetSizeSmoothness"), _tooltip);
            }
            EditorGUILayout.EndHorizontal();

            // Trigger targets
            EditorGUILayout.BeginHorizontal();
            _tooltip = new GUIContent("Use Targets Mid Point", "If checked, the trigger will use the mid point of all your camera targets");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseTargetsMidPoint"), _tooltip);

            if (!proCamera2DTriggerZoom.UseTargetsMidPoint)
            {
                _tooltip = new GUIContent("Trigger Target", "The target to use instead of the mid point of all camera targets");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TriggerTarget"), _tooltip);
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}