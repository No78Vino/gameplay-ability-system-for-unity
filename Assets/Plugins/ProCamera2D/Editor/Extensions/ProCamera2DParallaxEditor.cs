using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

#if USING_URP
using UnityEngine.Rendering.Universal;
#endif

namespace Com.LuisPedroFonseca.ProCamera2D
{
	[CustomEditor(typeof(ProCamera2DParallax))]
	public class ProCamera2DParallaxEditor : Editor
	{
		GUIContent _tooltip;

		ReorderableList _parallaxLayersList;

		ProCamera2D _proCamera2D;
		ProCamera2DParallax _proCamera2DParallax;

		MonoScript _script;
		
		void OnEnable()
		{
			if (target == null)
				return;

			_proCamera2DParallax = (ProCamera2DParallax)target;

			_proCamera2D = _proCamera2DParallax.ProCamera2D;

			_script = MonoScript.FromMonoBehaviour(_proCamera2DParallax);
			
			// Parallax layers List
			_parallaxLayersList = new ReorderableList(serializedObject, serializedObject.FindProperty("ParallaxLayers"), false, true, true, true);

			// Draw element callback
			_parallaxLayersList.drawElementCallback = (rect, index, isActive, isFocused) =>
			{
				rect.y += 2;
				var element = _parallaxLayersList.serializedProperty.GetArrayElementAtIndex(index);
				
				EditorGUI.PrefixLabel(new Rect(rect.x, rect.y, 65, 10), new GUIContent("Camera", "The parallax camera"));
				EditorGUI.PropertyField(new Rect(
						rect.x + 65,
						rect.y,
						80,
						EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("ParallaxCamera"), GUIContent.none);


				// Speed slider
				EditorGUI.LabelField(new Rect(rect.x + 170, rect.y, 65, 20), new GUIContent(_proCamera2DParallax.UseIndependentAxisSpeeds ? "SpeedX" : "Speed", "The relative speed at which the camera should move in comparison to the main camera."));
				
				if(_proCamera2DParallax.UseIndependentAxisSpeeds )
					EditorGUI.LabelField(new Rect(rect.x + 170, rect.y + 15, 65, 20), new GUIContent("SpeedY", "The relative speed at which the camera should move in comparison to the main camera."));
				
				if (_proCamera2DParallax.UseIndependentAxisSpeeds)
				{
					EditorGUI.PropertyField(new Rect(
							rect.x + 215,
							rect.y,
							rect.width - 215,
							EditorGUIUtility.singleLineHeight),
						element.FindPropertyRelative("SpeedX"), GUIContent.none);
					EditorGUI.PropertyField(new Rect(
							rect.x + 215,
							rect.y + 15,
							rect.width - 215,
							EditorGUIUtility.singleLineHeight),
						element.FindPropertyRelative("SpeedY"), GUIContent.none);
				}
				else
				{
					EditorGUI.PropertyField(new Rect(
							rect.x + 210,
							rect.y,
							rect.width - 210,
							EditorGUIUtility.singleLineHeight),
						element.FindPropertyRelative("Speed"), GUIContent.none);
				}

				if (_proCamera2DParallax.UseIndependentAxisSpeeds)
					rect.y += 10;

				// Layer mask
                EditorGUI.PrefixLabel(new Rect(rect.x, rect.y + 25, 65, 10), new GUIContent("Culling Mask", "Which layers should this camera render?"));
				EditorGUI.PropertyField(new Rect(
						rect.x + 85,
						rect.y + 25,
						rect.width - 85,
						EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("LayerMask"), GUIContent.none);
			};

			// Draw header callback
			_parallaxLayersList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Parallax layers");

			// Add element callback
			_parallaxLayersList.onAddCallback = list => AddParallaxLayer();

			// Remove element callback
			_parallaxLayersList.onRemoveCallback = list =>
			{
				if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this layer?", "Yes", "No"))
				{
					var cam = list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("ParallaxCamera").objectReferenceValue as Camera;
					if (cam != null)
						DestroyImmediate(cam.gameObject);

					ReorderableList.defaultBehaviours.DoRemoveButton(list);
				}
			};

			// Select element callback
			_parallaxLayersList.onSelectCallback = list =>
			{
				EditorGUIUtility.PingObject(list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("ParallaxCamera").objectReferenceValue as Camera);
			};

			_parallaxLayersList.onReorderCallback = list =>
			{
				if (_proCamera2DParallax.ParallaxLayers[0].ParallaxCamera.clearFlags != CameraClearFlags.SolidColor &&
					_proCamera2DParallax.ParallaxLayers[0].ParallaxCamera.clearFlags != CameraClearFlags.Skybox)
					_proCamera2DParallax.ParallaxLayers[0].ParallaxCamera.clearFlags = CameraClearFlags.SolidColor;
			};

			_parallaxLayersList.elementHeight = 70;
			_parallaxLayersList.headerHeight = 18;
			_parallaxLayersList.draggable = true;
		}

		public override void OnInspectorGUI()
		{
			if (target == null)
				return;

			serializedObject.Update();

			// Show script link
			GUI.enabled = false;
			_script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
			GUI.enabled = true;

			// ProCamera2D
			_tooltip = new GUIContent("Pro Camera 2D", "");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);

			if (_proCamera2DParallax.ProCamera2D == null)
				EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);

			_proCamera2D = _proCamera2DParallax.ProCamera2D;
			if (_proCamera2D == null)
			{
				serializedObject.ApplyModifiedProperties();
				return;
			}

			// Perspective camera
			if (!_proCamera2D.GameCamera.orthographic)
				EditorGUILayout.HelpBox("ProCamera2D Parallax needs an orthographic projection camera to work.", MessageType.Error, true);

			// Parallax layers List
			_parallaxLayersList.DoLayoutList();

			var areSpeedsNotOrdered = false;
			var isLayerMaskEmpty = false;
			var isLayerMaskOverlapping = false;
			for (var i = 0; i < _proCamera2DParallax.ParallaxLayers.Count; i++)
			{
				// Detect unordered speeds
				if (i < _proCamera2DParallax.ParallaxLayers.Count - 1)
				{
					if (_proCamera2DParallax.ParallaxLayers[i].Speed > _proCamera2DParallax.ParallaxLayers[i + 1].Speed)
						areSpeedsNotOrdered = true;
				}

				// Detect empty layer masks
				if (_proCamera2DParallax.ParallaxLayers[i].LayerMask.value == 0)
					isLayerMaskEmpty = true;

				// Detect equal layer masks
				for (var j = 0; j < _proCamera2DParallax.ParallaxLayers.Count; j++)
				{
					if (i != j && _proCamera2DParallax.ParallaxLayers[i].LayerMask.value == _proCamera2DParallax.ParallaxLayers[j].LayerMask.value)
						isLayerMaskOverlapping = true;
				}

				// Remove layers with no camera assigned
				if (_proCamera2DParallax.ParallaxLayers[i].ParallaxCamera == null)
				{
					_proCamera2DParallax.ParallaxLayers.RemoveAt(i);
					GUIUtility.ExitGUI();
					continue;
				}

				// Apply layer masks as camera culling mask
				_proCamera2DParallax.ParallaxLayers[i].ParallaxCamera.cullingMask = _proCamera2DParallax.ParallaxLayers[i].LayerMask;

				// Setup clear flags
				if (_proCamera2DParallax.ParallaxLayers[0].Speed > 1)
				{
					_proCamera2D.GameCamera.clearFlags = CameraClearFlags.SolidColor;
					_proCamera2DParallax.ParallaxLayers[i].ParallaxCamera.clearFlags = CameraClearFlags.Depth;
				}
				else
				{
					_proCamera2D.GameCamera.clearFlags = CameraClearFlags.Depth;

					if (i > 0)
					{
						_proCamera2DParallax.ParallaxLayers[i].ParallaxCamera.clearFlags = CameraClearFlags.Depth;
					}
					else if (_proCamera2DParallax.ParallaxLayers[0].ParallaxCamera.clearFlags != CameraClearFlags.SolidColor &&
					         _proCamera2DParallax.ParallaxLayers[0].ParallaxCamera.clearFlags != CameraClearFlags.Skybox &&
					         _proCamera2DParallax.AutomaticallyConfigureCameraClearFlags)
					{
						_proCamera2DParallax.ParallaxLayers[0].ParallaxCamera.clearFlags = CameraClearFlags.SolidColor;
					}
				}
			}

			// Show warnings
			if (areSpeedsNotOrdered)
				EditorGUILayout.HelpBox("Parallax layer speeds are not ordered.", MessageType.Warning, true);

			if (isLayerMaskEmpty)
				EditorGUILayout.HelpBox("One or more layer mask is empty.", MessageType.Warning, true);

			if (isLayerMaskOverlapping)
				EditorGUILayout.HelpBox("Two or more cameras are rendering the same layers. Check your Layer Masks.", MessageType.Warning, true);


			// Setup depths
			var sortedNegativeParallaxLayers = _proCamera2DParallax.ParallaxLayers
				.OrderByDescending(x => x.Speed)
				.Where(p => p.Speed < 1)
				.ToList();

			for (var i = 0; i < sortedNegativeParallaxLayers.Count; i++)
			{
				var parallaxLayer = sortedNegativeParallaxLayers[i];
				parallaxLayer.ParallaxCamera.depth = _proCamera2DParallax.BackDepthStart - i - 1;
			}

			var sortedPositiveParallaxLayers = _proCamera2DParallax.ParallaxLayers
				.OrderBy(x => x.Speed)
				.Where(p => p.Speed > 1)
				.ToList();

			for (var i = 0; i < sortedPositiveParallaxLayers.Count; i++)
			{
				var parallaxLayer = sortedPositiveParallaxLayers[i];
				parallaxLayer.ParallaxCamera.depth = _proCamera2DParallax.FrontDepthStart + i + 1;
			}
			
			// Setup hierarchy order
			var sortedParallaxLayers = _proCamera2DParallax.ParallaxLayers
				.OrderBy(x => x.Speed)
				.ToList();

			for (var i = 0; i < sortedParallaxLayers.Count; i++)
			{
				var parallaxLayer = sortedParallaxLayers[i];
				parallaxLayer.ParallaxCamera.transform.SetSiblingIndex(i);
			}
			
#if USING_URP
			sortedParallaxLayers.Add(new ProCamera2DParallaxLayer
			{
				ParallaxCamera = _proCamera2DParallax.ProCamera2D.GameCamera,
				CameraTransform = _proCamera2DParallax.ProCamera2D.transform,
				Speed = 1
			});
			
			sortedParallaxLayers = sortedParallaxLayers
				.OrderBy(x => x.Speed)
				.ToList();
			
			for (var i = 0; i < sortedParallaxLayers.Count; i++)
			{
				var parallaxLayer = sortedParallaxLayers[i];
				
				var cameraData = parallaxLayer.ParallaxCamera.GetUniversalAdditionalCameraData();
				if (cameraData == null) continue;
				
				cameraData.renderType = i == 0 ? CameraRenderType.Base : CameraRenderType.Overlay;

				if (i != 0 || sortedParallaxLayers.Count <= 1) continue;
				
				cameraData.cameraStack?.Clear();
				for (var j = 1; j < sortedParallaxLayers.Count; j++)
				{
					cameraData.cameraStack?.Add(sortedParallaxLayers[j].ParallaxCamera);
				}
			}
#endif

			// Show correct axis name
			var hAxis = "";
			var vAxis = "";
			switch (_proCamera2D.Axis)
			{
				case MovementAxis.XY:
					hAxis = "X";
					vAxis = "Y";
					break;

				case MovementAxis.XZ:
					hAxis = "X";
					vAxis = "Z";
					break;

				case MovementAxis.YZ:
					hAxis = "Y";
					vAxis = "Z";
					break;
			}

			// Follow X
			_tooltip = new GUIContent("Parallax " + hAxis, "Should the parallax occur on the horizontal axis? On by default.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ParallaxHorizontal"), _tooltip);

			// Follow Y
			_tooltip = new GUIContent("Parallax " + vAxis, "Should the parallax occur on the vertical axis? On by default.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ParallaxVertical"), _tooltip);

			// Parallax Zoom
			_tooltip = new GUIContent("Parallax Zoom", "Should the parallax layers size be affected? On by default.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ParallaxZoom"), _tooltip);
			
			// Use independent axis speeds
			_tooltip = new GUIContent("Use Independent Axis Speeds", "Enable if you want your parallax layers to move at different horizontal and vertical speeds. May cause inconsistent parallax results. Use only if you know what you're doing.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("UseIndependentAxisSpeeds"), _tooltip);
			
			// Automatically configure camera clear flags
			_tooltip = new GUIContent("Automatically Configure Clear Flags", "Disable only if you want to manually configure your camera clear flags.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("AutomaticallyConfigureCameraClearFlags"), _tooltip);

			// Back Depth Start
			_tooltip = new GUIContent("Back Depth Start", "The depth at which the back cameras start");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("BackDepthStart"), _tooltip);

			// Front Depth Start
			_tooltip = new GUIContent("Front Depth Start", "The depth at which the front cameras start");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("FrontDepthStart"), _tooltip);

			// Root Position
			_tooltip = new GUIContent("Root Position", "The position at which your camera is going to start.");
			EditorGUILayout.PropertyField(serializedObject.FindProperty("RootPosition"), _tooltip);

			// Reset offset button
			if (GUILayout.Button("Set Root Position"))
			{
				switch (_proCamera2D.Axis)
				{
					case MovementAxis.XY:
						_proCamera2DParallax.RootPosition = new Vector3(_proCamera2D.transform.localPosition.x, _proCamera2D.transform.localPosition.y, 0);
						break;

					case MovementAxis.XZ:
						_proCamera2DParallax.RootPosition = new Vector3(_proCamera2D.transform.localPosition.x, 0, _proCamera2D.transform.localPosition.z);
						break;

					case MovementAxis.YZ:
						_proCamera2DParallax.RootPosition = new Vector3(0, _proCamera2D.transform.localPosition.y, _proCamera2D.transform.localPosition.z);
						break;
				}
			}

			// Limit back and front depth start
			if (_proCamera2DParallax.FrontDepthStart < _proCamera2D.GameCamera.depth)
				_proCamera2DParallax.FrontDepthStart = (int)_proCamera2D.GameCamera.depth;

			if (_proCamera2DParallax.BackDepthStart > _proCamera2D.GameCamera.depth)
				_proCamera2DParallax.BackDepthStart = (int)_proCamera2D.GameCamera.depth;

			// Rename layers
			if (serializedObject.ApplyModifiedProperties())
			{
				foreach (var parallaxLayer in _proCamera2DParallax.ParallaxLayers)
				{
					parallaxLayer.ParallaxCamera.gameObject.name = "ParallaxCamera (" + parallaxLayer.Speed + ")";
				}
			}
		}

		private void AddParallaxLayer()
		{
			var go = new GameObject("ParallaxCamera");
			var cam = go.AddComponent<Camera>();
			EditorUtility.CopySerialized(_proCamera2D.GameCamera, cam);
			go.transform.SetParent(_proCamera2D.transform);
			go.transform.localPosition = Vector3.zero;

			cam.cullingMask = 0;
			cam.clearFlags = CameraClearFlags.Depth;

			var parallaxLayer = new ProCamera2DParallaxLayer
			{
				ParallaxCamera = cam, 
				Speed = 1
			};

			_proCamera2DParallax.ParallaxLayers.Add(parallaxLayer);
		}
	}
}