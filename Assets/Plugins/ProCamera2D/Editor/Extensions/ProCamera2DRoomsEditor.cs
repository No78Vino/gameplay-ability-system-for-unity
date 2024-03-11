using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditorInternal;

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[CustomEditor(typeof(ProCamera2DRooms))]
	public class ProCamera2DRoomsEditor : Editor
	{
		Func<Vector3, float> Vector3H;
		Func<Vector3, float> Vector3V;
		Func<float, float, Vector3> VectorHV;
		Func<float, float, float, Vector3> VectorHVD;

		MonoScript _script;
		GUIContent _tooltip;

		ReorderableList _roomsList;

		int _currentlyEditingRoom = -1;

		void OnEnable()
		{
			if (target == null)
				return;

			var proCamera2DRooms = (ProCamera2DRooms)target;
			
			if (proCamera2DRooms.ProCamera2D == null)
				return;

			// Add Numeric Boundaries component if needed
			var numericBoundaries = proCamera2DRooms.ProCamera2D.GetComponent<ProCamera2DNumericBoundaries>();
			if (numericBoundaries == null)
				proCamera2DRooms.ProCamera2D.gameObject.AddComponent<ProCamera2DNumericBoundaries>();

			// Create Vector conversion methods
			switch (proCamera2DRooms.ProCamera2D.Axis)
			{
				case MovementAxis.XY:
					Vector3H = vector => vector.x;
					Vector3V = vector => vector.y;
					VectorHV = (h, v) => new Vector3(h, v, 0);
					VectorHVD = (h, v, d) => new Vector3(h, v, d);
					break;
				case MovementAxis.XZ:
					Vector3H = vector => vector.x;
					Vector3V = vector => vector.z;
					VectorHV = (h, v) => new Vector3(h, 0, v);
					VectorHVD = (h, v, d) => new Vector3(h, d, v);
					break;
				case MovementAxis.YZ:
					Vector3H = vector => vector.z;
					Vector3V = vector => vector.y;
					VectorHV = (h, v) => new Vector3(0, v, h);
					VectorHVD = (h, v, d) => new Vector3(d, v, h);
					break;
			}

			// Script
			_script = MonoScript.FromMonoBehaviour(proCamera2DRooms);

			// Rooms List
			_roomsList = new ReorderableList(serializedObject, serializedObject.FindProperty("Rooms"), false, true, true, true);

			_roomsList.drawHeaderCallback = (Rect rect) =>
			{
				EditorGUI.LabelField(rect, "Rooms");
			};

			_roomsList.onSelectCallback = (list) =>
			{
				_currentlyEditingRoom = _roomsList.index;
				SceneView.RepaintAll();
			};

			_roomsList.onAddCallback = (list) =>
			{
				var index = list.serializedProperty.arraySize;
				list.serializedProperty.arraySize++;
				list.index = index;

				var newEntry = list.serializedProperty.GetArrayElementAtIndex(list.count - 1);
				if (list.count > 1)
				{
					var prevRect = list.serializedProperty.GetArrayElementAtIndex(list.count - 2).FindPropertyRelative("Dimensions").rectValue;
					prevRect.x += prevRect.width;
					newEntry.FindPropertyRelative("Dimensions").rectValue = prevRect;

				}
				else if (list.count == 1)
				{
					newEntry.FindPropertyRelative("Dimensions").rectValue = new Rect(0, 0, 10, 10);
					newEntry.FindPropertyRelative("TransitionDuration").floatValue = 1f;
					newEntry.FindPropertyRelative("ZoomScale").floatValue = 1.5f;
				}

				_currentlyEditingRoom = index;
			};

			_roomsList.onRemoveCallback = (list) =>
			{
				_currentlyEditingRoom = -1;
				SceneView.RepaintAll();
				ReorderableList.defaultBehaviours.DoRemoveButton(list);
			};

			_roomsList.drawElementCallback = (rect, index, isActive, isFocused) =>
			{
				rect.y += 2;
				var element = _roomsList.serializedProperty.GetArrayElementAtIndex(index);

				EditorGUI.DropShadowLabel(new Rect(rect.x, rect.y + 3, 60, 10), "Room " + index);
				rect.y += 20;

                EditorGUI.PrefixLabel(new Rect(rect.x, rect.y, 65, 10), new GUIContent("ID", "The room id"));
				EditorGUI.PropertyField(new Rect(
						rect.x + 80,
						rect.y,
						100,
						EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("ID"), GUIContent.none);

                EditorGUI.PrefixLabel(new Rect(rect.x + 200, rect.y, 65, 10), new GUIContent("Rect", "The room size and position"));
				EditorGUI.PropertyField(new Rect(
						rect.x + 260,
						rect.y,
						rect.width - 260,
						EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("Dimensions"), GUIContent.none);

                EditorGUI.PrefixLabel(new Rect(rect.x, rect.y + 45, 65, 10), new GUIContent("Ease Type", ""));
				EditorGUI.PropertyField(new Rect(
						rect.x + 80,
						rect.y + 45,
						100,
						EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("TransitionEaseType"), GUIContent.none);

                EditorGUI.PrefixLabel(new Rect(rect.x + 200, rect.y + 45, 65, 10), new GUIContent("Duration", "How long it takes for the camera to reach this room"));
				EditorGUI.PropertyField(new Rect(
						rect.x + 260,
						rect.y + 45,
						rect.width - 260,
						EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("TransitionDuration"), GUIContent.none);

                EditorGUI.PrefixLabel(new Rect(rect.x, rect.y + 70, 65, 10), new GUIContent("Scale To Fit", "If checked, the camera will resize to fit the room"));
				EditorGUI.PropertyField(new Rect(
						rect.x + 80,
						rect.y + 70,
						20,
						EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("ScaleCameraToFit"), GUIContent.none);

				GUI.enabled = !element.FindPropertyRelative("ScaleCameraToFit").boolValue;
				
                EditorGUI.PrefixLabel(new Rect(rect.x + 120, rect.y + 70, 65, 10), new GUIContent("Zoom", "If checked, the camera will zoom when entering the room"));
				EditorGUI.PropertyField(new Rect(
						rect.x + 160,
						rect.y + 70,
						20,
						EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("Zoom"), GUIContent.none);
				GUI.enabled = true;

				GUI.enabled = !element.FindPropertyRelative("ScaleCameraToFit").boolValue && element.FindPropertyRelative("Zoom").boolValue;
				
                EditorGUI.PrefixLabel(new Rect(rect.x + 200, rect.y + 70, 65, 10), new GUIContent("Scale", "The zoom scale"));
				EditorGUI.PropertyField(new Rect(
						rect.x + 260,
						rect.y + 70,
						rect.width - 260,
						EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("ZoomScale"), GUIContent.none);
				GUI.enabled = true;
			};

			_roomsList.elementHeight = 130;
			_roomsList.draggable = true;
		}

		public override void OnInspectorGUI()
		{
			if (target == null)
				return;

			var proCamera2DRooms = (ProCamera2DRooms)target;

			if (proCamera2DRooms.ProCamera2D == null)
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
			EditorGUILayout.Space();

			// Rooms List
			_roomsList.DoLayoutList();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			// Update interval
			_tooltip = new GUIContent("Update Interval", "Every X seconds detect collision. Smaller frequencies are more precise but also require more processing.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UpdateInterval"), _tooltip);

			if (proCamera2DRooms.UpdateInterval <= 0.01f)
				proCamera2DRooms.UpdateInterval = 0.01f;

			// Trigger targets
			EditorGUILayout.BeginHorizontal();
			_tooltip = new GUIContent("Use Targets Mid Point", "If checked, the trigger will use the mid point of all your camera targets");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseTargetsMidPoint"), _tooltip);

			if (!proCamera2DRooms.UseTargetsMidPoint)
			{
				_tooltip = new GUIContent("Trigger Target", "The target to use instead of the mid point of all camera targets");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("TriggerTarget"), _tooltip);
			}
			EditorGUILayout.EndHorizontal();

			// Transition instanly on start
			_tooltip = new GUIContent("Transition Instanly On Start", "If checked, the camera will instantly transition to the current room on start");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("TransitionInstanlyOnStart"), _tooltip);

			// Restore on room exit
			_tooltip = new GUIContent("Restore On Room Exit", "If checked, the camera will restore its initial settings when not in any room");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("RestoreOnRoomExit"), _tooltip);

			if (proCamera2DRooms.RestoreOnRoomExit)
			{
				EditorGUI.indentLevel = 1;

				// Restore duration
				_tooltip = new GUIContent("Restore Duration", "How long it takes to restore the original settings");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("RestoreDuration"), _tooltip);

				// Restore ease type
				_tooltip = new GUIContent("Restore Ease Type", "The ease type of the restore animation");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("RestoreEaseType"), _tooltip);

				EditorGUI.indentLevel = 0;
			}

			// Automatic room detection
			_tooltip = new GUIContent("Automatic Room Activation", "If checked, ProCamera2D will automatically transition to the rooms based on the target(s) position. If disabled, you have to manually call the method EnterRoom(roomID)");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("AutomaticRoomActivation"), _tooltip);
			
			// Use relative position - Option only visible if the gameobject is not the ProCamera2D container
			if (proCamera2DRooms.GetComponent<ProCamera2D>() == null)
			{
				_tooltip = new GUIContent("Use Relative Positioning", "If checked, the rooms positions are relative to the containing GameObject");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("UseRelativePosition"), _tooltip);
			}
			else
			{
				GUI.enabled = false;
				_tooltip = new GUIContent("Use Relative Positioning", "If checked, the rooms positions are relative to the containing GameObject. Only supported if the Rooms extension is on a GameObject different than the ProCamera2D core component");
				EditorGUILayout.PropertyField(serializedObject.FindProperty("UseRelativePosition"), _tooltip);
				GUI.enabled = true;
			}

			EditorGUILayout.Space();

			// Transition started event
			_tooltip = new GUIContent("OnStartedTransition", "");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStartedTransition"), _tooltip);

			// Transition started event
			_tooltip = new GUIContent("OnFinishedTransition", "");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("OnFinishedTransition"), _tooltip);

			// Exited all rooms event
			_tooltip = new GUIContent("OnExitedAllRooms", "This event fires when the camera target is not currently on any room");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("OnExitedAllRooms"), _tooltip);

			// Apply modified properties
			serializedObject.ApplyModifiedProperties();
		}

		void OnSceneGUI()
		{
			var proCamera2DRooms = (ProCamera2DRooms)target;
			
			if (proCamera2DRooms.ProCamera2D == null)
				return;

			// Text style for room numbers
			var guiStyle = new GUIStyle();
			guiStyle.fontStyle = FontStyle.Bold;
			guiStyle.fontSize = 30;
			guiStyle.normal.textColor = EditorPrefsX.GetColor(PrefsData.RoomsColorKey, PrefsData.RoomsColorValue);
			guiStyle.alignment = TextAnchor.MiddleCenter;
			guiStyle.fixedWidth = 1f;
			guiStyle.fixedHeight = 1f;

			Handles.color = EditorPrefsX.GetColor(PrefsData.RoomsColorKey, PrefsData.RoomsColorValue);
			for (int i = 0; i < proCamera2DRooms.Rooms.Count; i++)
			{
				Handles.color = new Color(Handles.color.r, Handles.color.g, Handles.color.b, 0f);

				// Button to toggle room editing
				var buttonSize = Mathf.Min(proCamera2DRooms.Rooms[i].Dimensions.width / 2f, proCamera2DRooms.Rooms[i].Dimensions.height / 2f);
				buttonSize = Mathf.Min(1, buttonSize);

				var buttonPosition = proCamera2DRooms.Rooms[i].Dimensions.position;
				if (proCamera2DRooms.UseRelativePosition)
				{
					buttonPosition.x += Vector3H(proCamera2DRooms.transform.position);
					buttonPosition.y += Vector3V(proCamera2DRooms.transform.position);
				}
				
				if (Handles.Button(
					buttonPosition,
					Quaternion.LookRotation(VectorHVD(0, 0, 1)),
					buttonSize,
					buttonSize,
					Handles.RectangleHandleCap))
				{
					if (i == _currentlyEditingRoom)
						_currentlyEditingRoom = -1;
					else
						_currentlyEditingRoom = i;
				}

				// Room number
				Handles.Label(VectorHV(buttonPosition.x, buttonPosition.y), i.ToString(), guiStyle);
			}

			// Room rect editor
			if (_currentlyEditingRoom != -1)
			{
				var currentDimensions = proCamera2DRooms.Rooms[_currentlyEditingRoom].Dimensions;

				// Snap value
				var snap = EditorPrefs.GetFloat("RoomsSnapping", PrefsData.RoomsSnapping);

				// Move
				var oldPos = VectorHV(currentDimensions.position.x, currentDimensions.position.y);
				if (proCamera2DRooms.UseRelativePosition)
				{
					oldPos.x += Vector3H(proCamera2DRooms.transform.position);
					oldPos.y += Vector3V(proCamera2DRooms.transform.position);
				}
				
				var newPos = Handles.PositionHandle(oldPos, Quaternion.identity);
				if (newPos != oldPos)
				{
					Undo.RecordObject(proCamera2DRooms, "MoveRoom");
					currentDimensions.position = new Vector2(Vector3H(newPos), Vector3V(newPos));
					
					if (proCamera2DRooms.UseRelativePosition)
					{
						currentDimensions.x -= Vector3H(proCamera2DRooms.transform.position);
						currentDimensions.y -= Vector3V(proCamera2DRooms.transform.position);
					}
				}

				// Draw rect editor
				var currentDimensionsRelative = currentDimensions;
				if (proCamera2DRooms.UseRelativePosition)
				{
					currentDimensionsRelative.x += Vector3H(proCamera2DRooms.transform.position);
					currentDimensionsRelative.y += Vector3V(proCamera2DRooms.transform.position);
				}
				
				var newDimensions = ResizeRect(
										currentDimensionsRelative,
										Handles.CubeHandleCap,
										Color.green,
										Color.yellow,
										HandleUtility.GetHandleSize(Vector3.zero) * .1f,
										snap);


				// Undo
				if (newDimensions.x != currentDimensionsRelative.x ||
					newDimensions.y != currentDimensionsRelative.y ||
					newDimensions.width != currentDimensionsRelative.width ||
					newDimensions.height != currentDimensionsRelative.height)
					Undo.RecordObject(proCamera2DRooms, "ResizeRect");

				// Save new dimensions
				var newDimensionsRelative = newDimensions;
				if (proCamera2DRooms.UseRelativePosition)
				{
					newDimensionsRelative.x -= Vector3H(proCamera2DRooms.transform.position);
					newDimensionsRelative.y -= Vector3V(proCamera2DRooms.transform.position);
				}
				
				proCamera2DRooms.Rooms[_currentlyEditingRoom].Dimensions = newDimensionsRelative;

				// Redraw views
				if (Event.current.rawType == EventType.Used)
					UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
			}
		}

		Rect ResizeRect(Rect rect, UnityEditor.Handles.CapFunction capFunc, Color capCol, Color fillCol, float capSize, float snap)
		{
			Vector2 halfSize = new Vector2(rect.size.x * 0.5f, rect.size.y * 0.5f);

			Vector3[] rectangleCorners =
				{
					VectorHVD(rect.position.x - halfSize.x, rect.position.y - halfSize.y, 0),   // Bottom Left
                    VectorHVD(rect.position.x + halfSize.x, rect.position.y - halfSize.y, 0),   // Bottom Right
                    VectorHVD(rect.position.x + halfSize.x, rect.position.y + halfSize.y, 0),   // Top Right
                    VectorHVD(rect.position.x - halfSize.x, rect.position.y + halfSize.y, 0)    // Top Left
                };

			Handles.color = fillCol;
			Handles.DrawSolidRectangleWithOutline(rectangleCorners, new Color(fillCol.r, fillCol.g, fillCol.b, 0.25f), capCol);

			Vector3[] handlePoints =
				{
					VectorHVD(rect.position.x - halfSize.x, rect.position.y, 0),   // Left
                    VectorHVD(rect.position.x + halfSize.x, rect.position.y, 0),   // Right
                    VectorHVD(rect.position.x, rect.position.y + halfSize.y, 0),   // Top
                    VectorHVD(rect.position.x, rect.position.y - halfSize.y, 0)    // Bottom 
                };

			Handles.color = capCol;

			var newSize = rect.size;
			var newPosition = rect.position;

			var leftHandle = Utils.AlignToGrid(Vector3H(Handles.Slider(handlePoints[0], -VectorHVD(1, 0, 0), capSize, capFunc, snap)) - Vector3H(handlePoints[0]), snap);
			var rightHandle = Utils.AlignToGrid(Vector3H(Handles.Slider(handlePoints[1], VectorHVD(1, 0, 0), capSize, capFunc, snap)) - Vector3H(handlePoints[1]), snap);
			var topHandle = Utils.AlignToGrid(Vector3V(Handles.Slider(handlePoints[2], VectorHVD(0, 1, 0), capSize, capFunc, snap)) - Vector3V(handlePoints[2]), snap);
			var bottomHandle = Utils.AlignToGrid(Vector3V(Handles.Slider(handlePoints[3], -VectorHVD(0, 1, 0), capSize, capFunc, snap)) - Vector3V(handlePoints[3]), snap);

			newSize = new Vector2(
				Mathf.Max(0f, newSize.x - leftHandle + rightHandle),
				Mathf.Max(0f, newSize.y + topHandle - bottomHandle));

			newPosition = new Vector2(
				newPosition.x + leftHandle * .5f + rightHandle * .5f,
				newPosition.y + topHandle * .5f + bottomHandle * .5f);

			return new Rect(newPosition.x, newPosition.y, newSize.x, newSize.y);
		}
	}
}