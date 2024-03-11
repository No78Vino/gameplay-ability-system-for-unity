using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DForwardFocus))]
    public class ProCamera2DForwardFocusEditor : Editor
    {
    	GUIContent _tooltip;

        MonoScript _script;

        void OnEnable()
        {
            if (target == null)
                return;
            
            _script = MonoScript.FromMonoBehaviour((ProCamera2DForwardFocus)target);
        }

    	public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            
        	var proCamera2DForwardFocus = (ProCamera2DForwardFocus)target;

			serializedObject.Update();

			// Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);

            if(proCamera2DForwardFocus.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);

			// Progressive
			_tooltip = new GUIContent("Progressive", "Should the forward focus move progressively based on the camera speed or should it transition to the focus based on the direction.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Progressive"), _tooltip);


            // Speed multiplier
            if(proCamera2DForwardFocus.Progressive)
            {
				_tooltip = new GUIContent("Speed Multiplier", "Multiply the camera speed by this number. You probably only want to change this value if the camera speed is not sufficient to reach the target focus.");
	            EditorGUILayout.PropertyField(serializedObject.FindProperty("SpeedMultiplier"), _tooltip);
            }


            // Transition smoothness
            _tooltip = new GUIContent("Transition Smoothness", "How smoothly should the forward focus influence move?");

            EditorGUILayout.PropertyField(serializedObject.FindProperty("TransitionSmoothness"), _tooltip);

            if(proCamera2DForwardFocus.TransitionSmoothness < 0f) proCamera2DForwardFocus.TransitionSmoothness = 0f;


            // Movement threshold
            _tooltip = new GUIContent ("Movement Threshold", "If the camera movement is smaller than this, it will be ignored. Use it to prevent unexpected forward focus direction changes");

            EditorGUILayout.PropertyField (serializedObject.FindProperty ("MovementThreshold"), _tooltip);

            if (proCamera2DForwardFocus.MovementThreshold.x < 0f) proCamera2DForwardFocus.MovementThreshold.x = 0f;
            if (proCamera2DForwardFocus.MovementThreshold.y < 0f) proCamera2DForwardFocus.MovementThreshold.y = 0f;


            // Maintain Influence On Stop
			_tooltip = new GUIContent("Maintain Influence On Stop", "Should the influence maintain after the camera stops?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("MaintainInfluenceOnStop"), _tooltip);


            // Left focus
            _tooltip = new GUIContent("Left Focus", "How much should the camera compensate when moving left?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LeftFocus"), _tooltip);


            // Right focus
            _tooltip = new GUIContent("Right Focus", "How much should the camera compensate when moving right?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RightFocus"), _tooltip);


            // Up focus
            _tooltip = new GUIContent("Up Focus", "How much should the camera compensate when moving up?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TopFocus"), _tooltip);


            // Down focus
            _tooltip = new GUIContent("Down Focus", "How much should the camera compensate when moving down?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BottomFocus"), _tooltip);


			serializedObject.ApplyModifiedProperties();
        }
    }
}