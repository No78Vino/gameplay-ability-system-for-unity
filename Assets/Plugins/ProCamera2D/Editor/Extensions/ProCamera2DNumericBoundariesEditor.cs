using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[CustomEditor(typeof(ProCamera2DNumericBoundaries))]
	public class ProCamera2DNumericBoundariesEditor : Editor
	{
		GUIContent _tooltip;

		MonoScript _script;

		void OnEnable()
		{
			if (target == null)
				return;

			_script = MonoScript.FromMonoBehaviour((ProCamera2DNumericBoundaries)target);
		}

		public override void OnInspectorGUI()
		{
			if (target == null)
				return;

			var proCamera2DNumericBoundaries = (ProCamera2DNumericBoundaries)target;
			if (proCamera2DNumericBoundaries.ProCamera2D == null)
			{
				EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);
				return;
			}

			serializedObject.Update();

			// Show script link
			GUI.enabled = false;
			_script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
			GUI.enabled = true;

			// ProCamera2D
			_tooltip = new GUIContent("Pro Camera 2D", "");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);

			_tooltip = new GUIContent("Use Numeric Boundaries", "Should the camera position be constrained by position?");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseNumericBoundaries"), _tooltip);

			// Boundaries
			GUI.enabled = proCamera2DNumericBoundaries.UseNumericBoundaries;
			EditorGUI.indentLevel = 1;

			EditorGUILayout.BeginHorizontal();
			_tooltip = new GUIContent("Use Top", "Prevent camera movement beyond this point");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseTopBoundary"), _tooltip);

			if (proCamera2DNumericBoundaries.UseTopBoundary)
			{
				_tooltip = new GUIContent(" ", "Prevent camera movement beyond this point");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("TopBoundary"), _tooltip);
			}

			if (proCamera2DNumericBoundaries.UseBottomBoundary && proCamera2DNumericBoundaries.TopBoundary < proCamera2DNumericBoundaries.BottomBoundary)
				proCamera2DNumericBoundaries.TopBoundary = proCamera2DNumericBoundaries.BottomBoundary;

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			_tooltip = new GUIContent("Use Bottom", "Prevent camera movement beyond this point");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseBottomBoundary"), _tooltip);

			if (proCamera2DNumericBoundaries.UseBottomBoundary)
			{
				_tooltip = new GUIContent(" ", "Prevent camera movement beyond this point");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("BottomBoundary"), _tooltip);
			}

			if (proCamera2DNumericBoundaries.UseTopBoundary && proCamera2DNumericBoundaries.BottomBoundary > proCamera2DNumericBoundaries.TopBoundary)
				proCamera2DNumericBoundaries.BottomBoundary = proCamera2DNumericBoundaries.TopBoundary;

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			_tooltip = new GUIContent("Use Left", "Prevent camera movement beyond this point");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseLeftBoundary"), _tooltip);

			if (proCamera2DNumericBoundaries.UseLeftBoundary)
			{
				_tooltip = new GUIContent(" ", "Prevent camera movement beyond this point");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("LeftBoundary"), _tooltip);
			}

			if (proCamera2DNumericBoundaries.UseRightBoundary && proCamera2DNumericBoundaries.LeftBoundary > proCamera2DNumericBoundaries.RightBoundary)
				proCamera2DNumericBoundaries.LeftBoundary = proCamera2DNumericBoundaries.RightBoundary;

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			_tooltip = new GUIContent("Use Right", "Prevent camera movement beyond this point");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseRightBoundary"), _tooltip);

			if (proCamera2DNumericBoundaries.UseRightBoundary)
			{
				_tooltip = new GUIContent(" ", "Prevent camera movement beyond this point");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("RightBoundary"), _tooltip);
			}

			if (proCamera2DNumericBoundaries.UseLeftBoundary && proCamera2DNumericBoundaries.RightBoundary < proCamera2DNumericBoundaries.LeftBoundary)
				proCamera2DNumericBoundaries.RightBoundary = proCamera2DNumericBoundaries.LeftBoundary;

			EditorGUILayout.EndHorizontal();

			if ((proCamera2DNumericBoundaries.UseTopBoundary && proCamera2DNumericBoundaries.UseBottomBoundary && proCamera2DNumericBoundaries.BottomBoundary == proCamera2DNumericBoundaries.TopBoundary) ||
				(proCamera2DNumericBoundaries.UseLeftBoundary && proCamera2DNumericBoundaries.UseRightBoundary && proCamera2DNumericBoundaries.LeftBoundary == proCamera2DNumericBoundaries.RightBoundary))
				EditorGUILayout.HelpBox("Same axis boundaries can't have the same value!", MessageType.Error, true);

			EditorGUI.indentLevel = 0;
			EditorGUILayout.Space();
			GUI.enabled = true;

			// Soft boundaries
			_tooltip = new GUIContent("Soft Boundaries", "If enabled, the camera movement will ease out at the boundaries instead of instantly stopping.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseSoftBoundaries"), _tooltip);

			GUI.enabled = proCamera2DNumericBoundaries.UseSoftBoundaries;
			EditorGUI.indentLevel = 1;
			
			EditorGUILayout.BeginHorizontal();
			_tooltip = new GUIContent("Frame Delay On Enter", "If enabled, the soft boundaries will be delayed for a number of frames (use if a jerking motion is noticed when entering a scene).");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseBoundaryDelayOnEnterScene"), _tooltip);

			GUI.enabled = proCamera2DNumericBoundaries.UseBoundaryDelayOnEnterScene;
			_tooltip = new GUIContent(" ", "The number of frames to delay the activation of the soft boundaries when entering scene.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("BoundaryDelayFrames"), _tooltip);
			EditorGUILayout.EndHorizontal();


			_tooltip = new GUIContent("Softness", "");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("Softness"), _tooltip);

			_tooltip = new GUIContent("Soft Area", "The distance from the boundary from which the camera will start easing out the movement");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("SoftAreaSize"), _tooltip);
			
			if (proCamera2DNumericBoundaries.SoftAreaSize <= 0)
				proCamera2DNumericBoundaries.SoftAreaSize = .001f;

			EditorGUI.indentLevel = 0;
			GUI.enabled = true;

			serializedObject.ApplyModifiedProperties();
		}
	}
}