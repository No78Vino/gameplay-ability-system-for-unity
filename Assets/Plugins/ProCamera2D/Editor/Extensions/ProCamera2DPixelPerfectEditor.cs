using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[CustomEditor(typeof(ProCamera2DPixelPerfect))]
	public class ProCamera2DPixelPerfectEditor : Editor
	{
		MonoScript _script;
		GUIContent _tooltip;

		void OnEnable()
		{
			if (target == null)
				return;

			_script = MonoScript.FromMonoBehaviour((ProCamera2DPixelPerfect)target);
		}

		public override void OnInspectorGUI()
		{
			if (target == null)
				return;

			var proCamera2DPixelPerfect = (ProCamera2DPixelPerfect)target;

			if (proCamera2DPixelPerfect.ProCamera2D == null)
			{
				EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);
				return;
			}

			var isOrthographic = proCamera2DPixelPerfect.ProCamera2D.GameCamera.orthographic;
			if (!isOrthographic)
				EditorGUILayout.HelpBox("Pixel perfect only works with orthographic cameras!", MessageType.Error, true);

			serializedObject.Update();

			// Show script link
			GUI.enabled = false;
			_script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
			GUI.enabled = true;

			// ProCamera2D
			_tooltip = new GUIContent("Pro Camera 2D", "");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);

			EditorGUI.BeginChangeCheck();

			// Pixels per unit
			_tooltip = new GUIContent("Pixels per Unit", "How many pixels in a sprite correspond to one unit in the world");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("PixelsPerUnit"), _tooltip);

			// Viewport auto-scale
			EditorGUILayout.BeginHorizontal();

			_tooltip = new GUIContent("Viewport AutoScale", "If not None, the camera will automatically calculate the best scale across all resolutions based on the art viewport.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ViewportAutoScale"), _tooltip);

			// Viewport Scale
			if (proCamera2DPixelPerfect.ViewportAutoScale != AutoScaleMode.None)
			{
				GUI.enabled = false;
				EditorGUILayout.LabelField(proCamera2DPixelPerfect.CalculateViewportScale() + "x", GUILayout.MaxWidth(60));
				GUI.enabled = true;
			}

			EditorGUILayout.EndHorizontal();

			// Viewport size in pixels
			if (proCamera2DPixelPerfect.ViewportAutoScale != AutoScaleMode.None)
			{
				_tooltip = new GUIContent("Game Viewport (pixels)", "Set it that if the screen was of this size, each pixel on the screen would correspond to a pixel on your art. On a pixel-art game this probably has low values.");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("TargetViewportSizeInPixels"), _tooltip);
			}

			// Zoom
			_tooltip = new GUIContent("Zoom", "The zoom level of the camera");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_zoom"), _tooltip);

			// Snap movement to grid
			EditorGUILayout.BeginHorizontal();
			_tooltip = new GUIContent("Snap Movement to Grid", "If checked, the the sprites will snap to the grid. Might create some stuttering on your camera targets, especially if you're using a large grid and a follow smoothness greater than zero.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("SnapMovementToGrid"), _tooltip);

			if (proCamera2DPixelPerfect.SnapMovementToGrid)
			{
				_tooltip = new GUIContent("Snap Camera", "If checked, the camera will also snap to the grid. If you notice some stuttering in your game elements try to leave this on.");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("SnapCameraToGrid"), _tooltip);
			}
			EditorGUILayout.EndHorizontal();

			// Draw grid
			EditorGUILayout.BeginHorizontal();

			_tooltip = new GUIContent("Draw Grid", "If checked, the camera will draw a pixel grid. 'Gizmos' button must be enabled on the Game window.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("DrawGrid"), _tooltip);

			if (proCamera2DPixelPerfect.DrawGrid)
			{
				_tooltip = new GUIContent("Grid Color", "");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("GridColor"), _tooltip);
			}

			EditorGUILayout.EndHorizontal();

			// Grid density warning
			if (proCamera2DPixelPerfect.DrawGrid && proCamera2DPixelPerfect.GridDensity < 4)
				EditorGUILayout.HelpBox("Grid density is too high to draw, so we're skipping it to avoid performance issues with the editor.", MessageType.None, true);

			// Save properties
			serializedObject.ApplyModifiedProperties();

			// Limit values
			if (proCamera2DPixelPerfect.PixelsPerUnit < 1f)
				proCamera2DPixelPerfect.PixelsPerUnit = 1f;

			if (proCamera2DPixelPerfect.TargetViewportSizeInPixels.x < 1)
				proCamera2DPixelPerfect.TargetViewportSizeInPixels.x = 1;

			if (proCamera2DPixelPerfect.TargetViewportSizeInPixels.y < 1)
				proCamera2DPixelPerfect.TargetViewportSizeInPixels.y = 1;

			// Resize camera
			if (EditorGUI.EndChangeCheck() || !Application.isPlaying)
			{
				proCamera2DPixelPerfect.ResizeCameraToPixelPerfect();
			}
		}
	}
}