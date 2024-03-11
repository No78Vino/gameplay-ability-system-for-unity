using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[CustomEditor(typeof(ProCamera2DPanAndZoom))]
	public class ProCamera2DPanAndZoomEditor : Editor
	{
		GUIContent _tooltip;

		MonoScript _script;

		void OnEnable()
		{
			if (target == null)
				return;

			_script = MonoScript.FromMonoBehaviour((ProCamera2DPanAndZoom)target);
		}

		public override void OnInspectorGUI()
		{
			if (target == null)
				return;

			var proCamera2DPanAndZoom = (ProCamera2DPanAndZoom)target;

			serializedObject.Update();

			// Show script link
			GUI.enabled = false;
			_script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
			GUI.enabled = true;

			// ProCamera2D
			_tooltip = new GUIContent("Pro Camera 2D", "");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);

			if (proCamera2DPanAndZoom.ProCamera2D == null)
				EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);

			EditorGUILayout.Space();
			
			// Input method
			_tooltip = new GUIContent("Automatic Input Detection", "If enabled, the input method will be automatically detected. Mouse on desktop and touch on mobile. Disable to enable both (beta).");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("AutomaticInputDetection"), _tooltip);

			if (!proCamera2DPanAndZoom.AutomaticInputDetection)
			{
				EditorGUI.indentLevel = 1;
				
				_tooltip = new GUIContent("Use Mouse Input", "");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("UseMouseInput"), _tooltip);
				
				_tooltip = new GUIContent("Use Touch Input", "");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("UseTouchInput"), _tooltip);
				
				EditorGUI.indentLevel = 0;

				EditorGUILayout.Space();
			}

			// Disable over uGUI
			_tooltip = new GUIContent("Disable Over uGUI", "If enabled, the camera won't pan or zoom if the user has a pointer (or touch) over a UI element");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("DisableOverUGUI"), _tooltip);

			// Allow Zoom
			_tooltip = new GUIContent("Allow Zoom", "If enabled, the user will be able to manually zoom the camera");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowZoom"), _tooltip);

			if (proCamera2DPanAndZoom.AllowZoom)
			{
				EditorGUI.indentLevel = 1;

				// Mouse zoom speed
				_tooltip = new GUIContent("Mouse Zoom Speed", "The speed at which to zoom when using the mouse wheel (Desktop Only)");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("MouseZoomSpeed"), _tooltip);

				// Pinch zoom speed
				_tooltip = new GUIContent("Pinch Zoom Speed", "The speed at which to zoom when using the pinch gesture (Mobile Only)");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("PinchZoomSpeed"), _tooltip);

				// Zoom smoothness
				_tooltip = new GUIContent("Zoom Smoothness", "The smoothness of the zoom");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("ZoomSmoothness"), _tooltip);

				// Max zoom in amount
				_tooltip = new GUIContent("Max Zoom In Amount", "Represents the maximum amount the camera should zoom in");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxZoomInAmount"), _tooltip);

				// Max zoom in amount
				_tooltip = new GUIContent("Max Zoom Out Amount", "Represents the maximum amount the camera should zoom out");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxZoomOutAmount"), _tooltip);

				// Zoom to input center
				_tooltip = new GUIContent("Zoom To Input Center", "If enabled, the camera will zoom to the current input position (mouse or touch). If disabled, the camera will zoom to its center");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("ZoomToInputCenter"), _tooltip);

				EditorGUI.indentLevel = 0;

				EditorGUILayout.Space();
			}

			// Allow Pan
			_tooltip = new GUIContent("Allow Pan", "If enabled, the user will be able to manually pan the camera");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowPan"), _tooltip);

			if (proCamera2DPanAndZoom.AllowPan)
			{
				EditorGUI.indentLevel = 1;

				// Use pan by drag
				_tooltip = new GUIContent("Use Drag Pan", "Pan the camera by dragging the 'world'");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("UsePanByDrag"), _tooltip);

				if (proCamera2DPanAndZoom.UsePanByDrag)
				{
					EditorGUI.indentLevel = 2;
					
					// Min Pan Amount
					_tooltip = new GUIContent("Min Pan Amount", "What's the minimum pan amount (in screen size percentage) that can trigger the pan movement? Consider it like a dead zone.");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("MinPanAmount"), _tooltip);
					
					// Pan mouse button
					_tooltip = new GUIContent("Drag Mouse Button", "Which mouse button do you want to use for panning? Only applicable if mouse is used");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("PanMouseButton"), _tooltip);
					
					_tooltip = new GUIContent("Drag Speed Multiplier", "The speed at which to pan the camera");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("DragPanSpeedMultiplier"), _tooltip);

					_tooltip = new GUIContent("Draggable Area", "A normalized screen space area where the drag is active. Leave to default to use the whole screen");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("DraggableAreaRect"), _tooltip);

					_tooltip = new GUIContent("Stop Speed On Drag Start", "How fast the camera inertia stops once the user starts dragging");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("StopSpeedOnDragStart"), _tooltip);
					EditorGUI.indentLevel = 1;
				}

				// Use pan by move to edges
				_tooltip = new GUIContent("Use Edges Pan", "Pan the camera by moving the mouse to the edges of the screen (Desktop only)");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("UsePanByMoveToEdges"), _tooltip);

				if (proCamera2DPanAndZoom.UsePanByMoveToEdges)
				{
					EditorGUI.indentLevel = 2;
					_tooltip = new GUIContent("Edges Pan Speed", "The speed at which the camera will move when the mouse reaches the edges of the screen");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("EdgesPanSpeed"), _tooltip);

					_tooltip = new GUIContent("Top Pan Edge", "If the mouse pointer goes beyond this edge the camera will start moving vertically");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("TopPanEdge"), _tooltip);

					_tooltip = new GUIContent("Bottom Pan Edge", "If the mouse pointer goes beyond this edge the camera will start moving vertically");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("BottomPanEdge"), _tooltip);

					_tooltip = new GUIContent("Left Pan Edge", "If the mouse pointer goes beyond this edge the camera will start moving horizontally");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("LeftPanEdge"), _tooltip);

					_tooltip = new GUIContent("Right Pan Edge", "If the mouse pointer goes beyond this edge the camera will start moving horizontally");
					EditorGUILayout.PropertyField(serializedObject.FindProperty("RightPanEdge"), _tooltip);
					EditorGUI.indentLevel = 1;
				}

				EditorGUI.indentLevel = 0;
			}

			// Apply properties
			serializedObject.ApplyModifiedProperties();

			// Limit
			if (proCamera2DPanAndZoom.MaxZoomInAmount < 1f)
				proCamera2DPanAndZoom.MaxZoomInAmount = 1f;

			if (proCamera2DPanAndZoom.MaxZoomOutAmount < 1f)
				proCamera2DPanAndZoom.MaxZoomOutAmount = 1f;

			if (proCamera2DPanAndZoom.ZoomSmoothness < 0f)
				proCamera2DPanAndZoom.ZoomSmoothness = 0f;

			if (proCamera2DPanAndZoom.DraggableAreaRect.width < 0f)
				proCamera2DPanAndZoom.DraggableAreaRect.width = 0f;

			if (proCamera2DPanAndZoom.DraggableAreaRect.width > 1f)
				proCamera2DPanAndZoom.DraggableAreaRect.width = 1f;

			if (proCamera2DPanAndZoom.DraggableAreaRect.height < 0f)
				proCamera2DPanAndZoom.DraggableAreaRect.height = 0f;

			if (proCamera2DPanAndZoom.DraggableAreaRect.height > 1f)
				proCamera2DPanAndZoom.DraggableAreaRect.height = 1f;

			if (proCamera2DPanAndZoom.DraggableAreaRect.x < -.5f)
				proCamera2DPanAndZoom.DraggableAreaRect.x = -.5f;

			if (proCamera2DPanAndZoom.DraggableAreaRect.x > .5f)
				proCamera2DPanAndZoom.DraggableAreaRect.x = .5f;

			if (proCamera2DPanAndZoom.DraggableAreaRect.y < -.5f)
				proCamera2DPanAndZoom.DraggableAreaRect.y = -.5f;

			if (proCamera2DPanAndZoom.DraggableAreaRect.y > .5f)
				proCamera2DPanAndZoom.DraggableAreaRect.y = .5f;
			
			if (proCamera2DPanAndZoom.MinPanAmount < 0f)
				proCamera2DPanAndZoom.MinPanAmount = 0f;
			
			if (proCamera2DPanAndZoom.MinPanAmount > 1f)
				proCamera2DPanAndZoom.MinPanAmount = 1f;

			// Warning
			if (!proCamera2DPanAndZoom.AllowPan && !proCamera2DPanAndZoom.AllowZoom)
			{
				EditorGUILayout.HelpBox("This won't be the most useful extension now, will it?", MessageType.Warning, true);
			}
		}
	}
}