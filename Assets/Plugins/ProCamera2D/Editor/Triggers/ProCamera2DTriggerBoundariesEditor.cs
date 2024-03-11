using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[CustomEditor(typeof(ProCamera2DTriggerBoundaries))]
	[CanEditMultipleObjects]
    public class ProCamera2DTriggerBoundariesEditor : Editor
    {
        MonoScript _script;
    	GUIContent _tooltip;

        void OnEnable()
        {
			var proCamera2DTriggerBoundaries = (ProCamera2DTriggerBoundaries)target;

            if (proCamera2DTriggerBoundaries.NumericBoundaries == null && proCamera2DTriggerBoundaries.ProCamera2D != null)
            {
                var numericBoundaries = proCamera2DTriggerBoundaries.ProCamera2D.GetComponent<ProCamera2DNumericBoundaries>();
                proCamera2DTriggerBoundaries.NumericBoundaries = numericBoundaries == null ? proCamera2DTriggerBoundaries.ProCamera2D.gameObject.AddComponent<ProCamera2DNumericBoundaries>() : numericBoundaries;
            }

            _script = MonoScript.FromMonoBehaviour(proCamera2DTriggerBoundaries);
        }

        public override void OnInspectorGUI()
        {
			var proCamera2DTriggerBoundaries = (ProCamera2DTriggerBoundaries)target;

            if (proCamera2DTriggerBoundaries.ProCamera2D == null)
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

            if (proCamera2DTriggerBoundaries.UpdateInterval <= 0.01f)
                proCamera2DTriggerBoundaries.UpdateInterval = 0.01f;

            // Trigger shape
            _tooltip = new GUIContent("Trigger Shape", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TriggerShape"), _tooltip);

			// Boundaries relative
			_tooltip = new GUIContent("Are boundaries relative", "Are the boundaries relative to this or are they world positions?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AreBoundariesRelative"), _tooltip);			

			// Top boundary
			EditorGUILayout.BeginHorizontal();

			_tooltip = new GUIContent("Use Top Boundary", "Should the camera top position be limited?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseTopBoundary"), _tooltip);

            if(proCamera2DTriggerBoundaries.UseTopBoundary)
            {
	            _tooltip = new GUIContent(" ", "Camera top boundary");
	            EditorGUILayout.PropertyField(serializedObject.FindProperty("TopBoundary"), _tooltip);
            }

            EditorGUILayout.EndHorizontal();

            // Bottom boundary
            EditorGUILayout.BeginHorizontal();

			_tooltip = new GUIContent("Use Bottom Boundary", "Should the camera bottom position be limited?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseBottomBoundary"), _tooltip);

            if(proCamera2DTriggerBoundaries.UseBottomBoundary)
            {
	            _tooltip = new GUIContent(" ", "Camera bottom boundary");
	            EditorGUILayout.PropertyField(serializedObject.FindProperty("BottomBoundary"), _tooltip);
            }

            EditorGUILayout.EndHorizontal();

            // Left boundary
            EditorGUILayout.BeginHorizontal();

			_tooltip = new GUIContent("Use Left Boundary", "Should the camera left position be limited?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseLeftBoundary"), _tooltip);

            if(proCamera2DTriggerBoundaries.UseLeftBoundary)
            {
	            _tooltip = new GUIContent(" ", "Camera left boundary");
	            EditorGUILayout.PropertyField(serializedObject.FindProperty("LeftBoundary"), _tooltip);
            }

            EditorGUILayout.EndHorizontal();

            // Right boundary
            EditorGUILayout.BeginHorizontal();

			_tooltip = new GUIContent("Use Right Boundary", "Should the camera right position be limited?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseRightBoundary"), _tooltip);

            if(proCamera2DTriggerBoundaries.UseRightBoundary)
            {
	            _tooltip = new GUIContent(" ", "Camera right boundary");
	            EditorGUILayout.PropertyField(serializedObject.FindProperty("RightBoundary"), _tooltip);
            }

            EditorGUILayout.EndHorizontal();

            // Transition duration
            _tooltip = new GUIContent("Transition duration", "How many X seconds should the transition take?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TransitionDuration"), _tooltip);

            if (proCamera2DTriggerBoundaries.TransitionDuration <= 0)
                proCamera2DTriggerBoundaries.TransitionDuration = 0;

            // Transition ease type
            _tooltip = new GUIContent("Transition ease type", "The transition animation ease type");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TransitionEaseType"), _tooltip);

            // Trigger targets
            EditorGUILayout.BeginHorizontal();
            _tooltip = new GUIContent("Use Targets Mid Point", "If checked, the trigger will use the mid point of all your camera targets");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseTargetsMidPoint"), _tooltip);

            if (!proCamera2DTriggerBoundaries.UseTargetsMidPoint)
            {
                _tooltip = new GUIContent("Trigger Target", "The target to use instead of the mid point of all camera targets");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TriggerTarget"), _tooltip);
            }
            EditorGUILayout.EndHorizontal();

            // Change zoom
            _tooltip = new GUIContent("Change Zoom", "Change the camera zoom in/out?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ChangeZoom"), _tooltip);

            if (proCamera2DTriggerBoundaries.ChangeZoom)
            {
                // Target zoom
                _tooltip = new GUIContent("Zoom Amount", "");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("TargetZoom"), _tooltip);

                // Zoom smoothness
                _tooltip = new GUIContent("Zoom Smoothness", "");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ZoomSmoothness"), _tooltip);
            }

            // Set as starting boundaries
            _tooltip = new GUIContent("Set as starting boundaries", "Use this boundaries as the starting ones");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_setAsStartingBoundaries"), _tooltip);

            if(proCamera2DTriggerBoundaries._setAsStartingBoundaries)
            {
                var allBoundariesTriggers = FindObjectsOfType(typeof(ProCamera2DTriggerBoundaries));
                foreach (ProCamera2DTriggerBoundaries trigger in allBoundariesTriggers) 
                {
                    trigger._setAsStartingBoundaries = false;
                }

                proCamera2DTriggerBoundaries._setAsStartingBoundaries = true;
            }

            // Limit boundaries
			if (proCamera2DTriggerBoundaries.LeftBoundary > proCamera2DTriggerBoundaries.RightBoundary)
				proCamera2DTriggerBoundaries.LeftBoundary = proCamera2DTriggerBoundaries.RightBoundary;

			if (proCamera2DTriggerBoundaries.RightBoundary < proCamera2DTriggerBoundaries.LeftBoundary)
				proCamera2DTriggerBoundaries.RightBoundary = proCamera2DTriggerBoundaries.LeftBoundary;

			if (proCamera2DTriggerBoundaries.BottomBoundary > proCamera2DTriggerBoundaries.TopBoundary)
				proCamera2DTriggerBoundaries.BottomBoundary = proCamera2DTriggerBoundaries.TopBoundary;

			if (proCamera2DTriggerBoundaries.TopBoundary < proCamera2DTriggerBoundaries.BottomBoundary)
				proCamera2DTriggerBoundaries.TopBoundary = proCamera2DTriggerBoundaries.BottomBoundary;


			serializedObject.ApplyModifiedProperties();
        }
    }
}