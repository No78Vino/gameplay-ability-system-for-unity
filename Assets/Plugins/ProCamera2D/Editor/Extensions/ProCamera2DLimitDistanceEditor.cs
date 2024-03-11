using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[CustomEditor(typeof(ProCamera2DLimitDistance))]
	public class ProCamera2DLimitDistanceEditor : Editor
	{
		GUIContent _tooltip;

		MonoScript _script;

		void OnEnable()
		{
			if (target == null)
				return;

			_script = MonoScript.FromMonoBehaviour((ProCamera2DLimitDistance)target);
		}

		public override void OnInspectorGUI()
		{
			if (target == null)
				return;

			var proCamera2DLimitDistance = (ProCamera2DLimitDistance)target;

			serializedObject.Update();

			// Show script link
			GUI.enabled = false;
			_script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
			GUI.enabled = true;

			// ProCamera2D
			_tooltip = new GUIContent("Pro Camera 2D", "");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);

			if (proCamera2DLimitDistance.ProCamera2D == null)
				EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);
			
			// Use Targets Position
			EditorGUILayout.Space();
			_tooltip = new GUIContent("Use Targets Position", "If enabled, the extension will use the targets midpoint instead of the camera center for calculations.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseTargetsPosition"), _tooltip);
			EditorGUILayout.Space();

			// Limits
			DrawGUI(
				"Limit Top Distance",
				"Prevent the camera target from getting out of the screen. Use this if you have a high follow smoothness and your targets are getting out of the screen.",
				"LimitTopCameraDistance",
				proCamera2DLimitDistance.LimitTopCameraDistance,
				"MaxTopTargetDistance");

			DrawGUI(
				"Limit Bottom Distance",
				"Prevent the camera target from getting out of the screen. Use this if you have a high follow smoothness and your targets are getting out of the screen.",
				"LimitBottomCameraDistance",
				proCamera2DLimitDistance.LimitBottomCameraDistance,
				"MaxBottomTargetDistance");

			DrawGUI(
				"Limit Left Distance",
				"Prevent the camera target from getting out of the screen. Use this if you have a high follow smoothness and your targets are getting out of the screen.",
				"LimitLeftCameraDistance",
				proCamera2DLimitDistance.LimitLeftCameraDistance,
				"MaxLeftTargetDistance");

			DrawGUI(
				"Limit Right Distance",
				"Prevent the camera target from getting out of the screen. Use this if you have a high follow smoothness and your targets are getting out of the screen.",
				"LimitRightCameraDistance",
				proCamera2DLimitDistance.LimitRightCameraDistance,
				"MaxRightTargetDistance");

			serializedObject.ApplyModifiedProperties();
		}

		void DrawGUI(string label, string description, string prop1, bool prop2, string prop3)
		{
			EditorGUILayout.BeginHorizontal();

			_tooltip = new GUIContent(label, description);
			EditorGUILayout.PropertyField(serializedObject.FindProperty(prop1), _tooltip);

			if (prop2)
			{
				_tooltip = new GUIContent(" ", "");
				EditorGUILayout.PropertyField(serializedObject.FindProperty(prop3), _tooltip);
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}