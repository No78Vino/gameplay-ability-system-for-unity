using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public class PC2DEditorExtension
    {
        public string ExtName;
        public System.Type ExtType;
        public BasePC2D Component;
    }

    public class PC2DEditorTrigger
    {
        public string TriggerName;
        public System.Type TriggerType;
        public List<Object> AllTriggers;
        public int TriggerCurrentIndex;
    }

    [CustomEditor(typeof(ProCamera2D))]
    public class ProCamera2DEditor : Editor
    {
        GUIContent _tooltip;

        ReorderableList _targetsList;

        List<PC2DEditorExtension> _extensions;
        List<PC2DEditorTrigger> _triggers;
        
        string hAxis = "";
        string vAxis = "";

        bool _showReviewMessage;

        void OnEnable()
        {
            var proCamera2D = (ProCamera2D)target;

            if (proCamera2D.GameCamera == null)
                proCamera2D.GameCamera = proCamera2D.GetComponent<Camera>();
            
            // Show correct axis name
            switch (proCamera2D.Axis)
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

            // Targets List
            _targetsList = new ReorderableList(serializedObject, serializedObject.FindProperty("CameraTargets"), false, false, true, true);

            _targetsList.onSelectCallback = (list) =>
            {
                EditorGUIUtility.PingObject(_targetsList.serializedProperty.GetArrayElementAtIndex(_targetsList.index).FindPropertyRelative("TargetTransform").objectReferenceValue);
            };

            _targetsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                var element = _targetsList.serializedProperty.GetArrayElementAtIndex(index);

				EditorGUI.PrefixLabel(new Rect(rect.x, rect.y, 65, 10), new GUIContent("Transform", "The target transform"));
                EditorGUI.PropertyField(new Rect(
                        rect.x + 65,
                        rect.y,
                        80,
                        EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("TargetTransform"), GUIContent.none);

				EditorGUI.PrefixLabel(new Rect(rect.x + 160, rect.y, 65, 10), new GUIContent("Offset", "Offset the camera position relative to this target"));
                EditorGUI.PropertyField(new Rect(
                        rect.x + 200,
                        rect.y,
                        rect.width - 200,
                        EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("TargetOffset"), GUIContent.none);

                EditorGUI.PrefixLabel(new Rect(rect.x, rect.y + 25, 65, 10), new GUIContent("Influence" + hAxis, "How much does this target horizontal position influences the camera position?"));
                EditorGUI.PropertyField(new Rect(
                        rect.x + 80,
                        rect.y + 25,
                        rect.width - 80,
                        EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("TargetInfluenceH"), GUIContent.none);

                EditorGUI.PrefixLabel(new Rect(rect.x, rect.y + 40, 65, 10), new GUIContent("Influence" + vAxis, "How much does this target vertical position influences the camera position?"));
                EditorGUI.PropertyField(new Rect(
                        rect.x + 80,
                        rect.y + 40,
                        rect.width - 80,
                        EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("TargetInfluenceV"), GUIContent.none);
            };

            _targetsList.elementHeight = 80;
            _targetsList.headerHeight = 0;
            _targetsList.draggable = true;


            // Review message
            if (EditorPrefs.GetInt("ProCamera2DReview", 0) >= 0)
                EditorPrefs.SetInt("ProCamera2DReview", EditorPrefs.GetInt("ProCamera2DReview", 0) + 1);

            if (EditorPrefs.GetInt("ProCamera2DReview", 0) >= 100)
                _showReviewMessage = true;



            // Get extensions and triggers to show on the list
            _extensions = new List<PC2DEditorExtension>();
            _triggers = new List<PC2DEditorTrigger>();
            MonoScript[] scripts = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));
            foreach (MonoScript m in scripts)
            {
                var scriptClass = m.GetClass();
                if (scriptClass != null && scriptClass.IsSubclassOf(typeof(BasePC2D)))
                {
                    var extensionName = scriptClass.GetField("ExtensionName");
                    if (extensionName != null)
                    {
                        var ext = new PC2DEditorExtension()
                        {
                            ExtName = extensionName.GetValue(null) as string,
                            ExtType = scriptClass,
                            Component = proCamera2D.GetComponent(scriptClass) as BasePC2D
                        };
                        
                        _extensions.Add(ext);
                    }

                    var triggerName = scriptClass.GetField("TriggerName");
                    if (triggerName != null)
                    {
                        var trig = new PC2DEditorTrigger()
                            {
                                TriggerName = triggerName.GetValue(null) as string,
                                TriggerType = scriptClass,
                                AllTriggers = FindObjectsOfType(scriptClass).ToList(),
                                TriggerCurrentIndex = 0
                            };

                        _triggers.Add(trig);
                    }
                }
            }
            _extensions = _extensions.OrderBy(s => s.ExtName).ToList();
            _triggers = _triggers.OrderBy(s => s.TriggerName).ToList();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            var proCamera2D = (ProCamera2D)target;

            serializedObject.Update();


            EditorGUILayout.Space();

            // Draw User Guide link
            if (ProCamera2DEditorResources.UserGuideIcon != null)
            {
                var rect = GUILayoutUtility.GetRect(0f, 0f);
                rect.width = ProCamera2DEditorResources.UserGuideIcon.width;
                rect.height = ProCamera2DEditorResources.UserGuideIcon.height;
                if (GUI.Button(new Rect(15, rect.y, 32, 32), new GUIContent(ProCamera2DEditorResources.UserGuideIcon, "User Guide")))
                {
                    Application.OpenURL("http://www.procamera2d.com/user-guide/");
                }
            }

            // Draw header
            if (ProCamera2DEditorResources.InspectorHeader != null)
            {
                var rect = GUILayoutUtility.GetRect(0f, 0f);
                rect.x += 37;
                rect.width = ProCamera2DEditorResources.InspectorHeader.width;
                rect.height = ProCamera2DEditorResources.InspectorHeader.height;
                GUILayout.Space(rect.height);
                GUI.DrawTexture(rect, ProCamera2DEditorResources.InspectorHeader);
            }

            EditorGUILayout.Space();


            // Review prompt
            if (_showReviewMessage)
            {
                EditorGUILayout.HelpBox("Sorry for the intrusion, but I need you for a second. Reviews on the Asset Store are very important for me to keep improving this product. Please take a minute to write a review and support ProCamera2D.", MessageType.Warning, true);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Sure, I'll write a review!"))
                {
                    _showReviewMessage = false;
                    EditorPrefs.SetInt("ProCamera2DReview", -1);
                    Application.OpenURL("http://u3d.as/i7L");
                }
                if (GUILayout.Button("No, thanks."))
                {
                    _showReviewMessage = false;
                    EditorPrefs.SetInt("ProCamera2DReview", -1);
                }
                EditorGUILayout.EndHorizontal();
                AddSpace();
            }


            // Assign game camera
            if (proCamera2D.GameCamera == null)
                proCamera2D.GameCamera = proCamera2D.GetComponent<Camera>();


            // Targets Drop Area
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            var style = new GUIStyle("box");
            if (EditorGUIUtility.isProSkin)
                style.normal.textColor = Color.white;
            GUI.Box(drop_area, "\nDROP CAMERA TARGETS HERE", style);
            
            Undo.RecordObject(proCamera2D, "Added camera targets");
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object dragged_object in DragAndDrop.objectReferences)
                        {
                            var newCameraTarget = new CameraTarget
                            {
                                TargetTransform = ((GameObject)dragged_object).transform,
                                TargetInfluence = 1f
                            };
							
                            proCamera2D.CameraTargets.Add(newCameraTarget);
                            EditorUtility.SetDirty(proCamera2D);
                        }
                    }
                    break;
            }

            EditorGUILayout.Space();




            // Remove empty targets
            for (int i = 0; i < proCamera2D.CameraTargets.Count; i++)
            {
                if (proCamera2D.CameraTargets[i].TargetTransform == null)
                {
                    proCamera2D.CameraTargets.RemoveAt(i);
                }
            }




            // Targets List
            _targetsList.DoLayoutList();
            AddSpace();





            // Center target on start
            _tooltip = new GUIContent("Center target on start", "Should the camera move instantly to the target on game start?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CenterTargetOnStart"), _tooltip);

            AddSpace();
            




            // Axis
            _tooltip = new GUIContent("Axis", "Choose the axis in which the camera should move.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Axis"), _tooltip);

            AddSpace();




            // UpdateType
            _tooltip = new GUIContent("Update Type", "LateUpdate: Non physics based game\nFixedUpdate: Physics based game");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UpdateType"), _tooltip);

            AddSpace();




            // Show correct axis name
            switch (proCamera2D.Axis)
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




            // Follow horizontal
            EditorGUILayout.BeginHorizontal();
            _tooltip = new GUIContent("Follow " + hAxis, "Should the camera move on the horizontal axis?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FollowHorizontal"), _tooltip);
            if (proCamera2D.FollowHorizontal)
            {
                // Follow smoothness
                _tooltip = new GUIContent("Smoothness", "How long it takes the camera to reach the target horizontal position.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("HorizontalFollowSmoothness"), _tooltip);

                if (proCamera2D.HorizontalFollowSmoothness < 0f)
                    proCamera2D.HorizontalFollowSmoothness = 0f;

            }
            EditorGUILayout.EndHorizontal();


            // Follow vertical
            EditorGUILayout.BeginHorizontal();
            _tooltip = new GUIContent("Follow " + vAxis, "Should the camera move on the vertical axis?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FollowVertical"), _tooltip);

            if (proCamera2D.FollowVertical)
            {
                // Follow smoothness
                _tooltip = new GUIContent("Smoothness", "How long it takes the camera to reach the target vertical position.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("VerticalFollowSmoothness"), _tooltip);

                if (proCamera2D.VerticalFollowSmoothness < 0f)
                    proCamera2D.VerticalFollowSmoothness = 0f;
            }
            EditorGUILayout.EndHorizontal();

            if (!proCamera2D.FollowHorizontal && !proCamera2D.FollowVertical)
                EditorGUILayout.HelpBox("Camera won't move if it's not following the targets on any axis.", MessageType.Error, true);

            AddSpace();





            // Overall offset
            EditorGUILayout.LabelField("Offset");
            EditorGUI.indentLevel = 1;

            if (proCamera2D.IsRelativeOffset)
            {
                _tooltip = new GUIContent(hAxis, "Horizontal offset");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OffsetX"), _tooltip);

                _tooltip = new GUIContent(vAxis, "Vertical offset");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OffsetY"), _tooltip);
            }
            else
            {
                _tooltip = new GUIContent(hAxis, "Horizontal offset");
                serializedObject.FindProperty("OffsetX").floatValue = EditorGUILayout.FloatField(hAxis, serializedObject.FindProperty("OffsetX").floatValue);

                _tooltip = new GUIContent(vAxis, "Vertical offset");
                serializedObject.FindProperty("OffsetY").floatValue = EditorGUILayout.FloatField(vAxis, serializedObject.FindProperty("OffsetY").floatValue);
            }

            _tooltip = new GUIContent("Relative Offset", "If enabled, the offset is relative to the current screen size. Otherwise it's in world units.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsRelativeOffset"), _tooltip);

            EditorGUI.indentLevel = 0;

            AddSpace();



            // Zoom with FOV
            if (!proCamera2D.GameCamera.orthographic)
            {
                GUI.enabled = !Application.isPlaying;
                _tooltip = new GUIContent("Zoom With FOV", "If enabled, when using a perspective camera, the camera will zoom by changing the FOV instead of moving the camera closer/further from the objects.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ZoomWithFOV"), _tooltip);
                GUI.enabled = true;
                AddSpace();
            }
            
            
            
            // Ignore TimeScale
            _tooltip = new GUIContent("Ignore TimeScale", "If enabled, the camera will not be affected by the Time.timeScale. It will use Time.unscaledDeltaTime instead.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IgnoreTimeScale"), _tooltip);
            AddSpace();



            // Divider
            GUILayout.Box("", new GUILayoutOption[]{ GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            GUILayout.Label("EXTENSIONS", EditorStyles.boldLabel);
            EditorGUILayout.Space();


            // Extensions
            GUI.color = Color.white;
            for (int i = 0; i < _extensions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(_extensions[i].ExtName);
                if (_extensions[i].Component == null)
                {
                    GUI.color = new Color(133f / 255f, 235f / 255f, 154f / 255f, 1f);
                    if (GUILayout.Button("Enable"))
                    {
                        _extensions[i].Component = proCamera2D.gameObject.AddComponent(_extensions[i].ExtType) as BasePC2D;
                    }
                }
                else
                {
                    GUI.color = new Color(236f / 255f, 72f / 255f, 105f / 255f, 1f);
                    if (GUILayout.Button("Disable"))
                    {
                        if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to remove this extension?", "Yes", "No"))
                        {
                            DestroyImmediate(_extensions[i].Component);
                            EditorGUIUtility.ExitGUI();
                        }
                    }
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }



            // Divider
            EditorGUILayout.Space();
            GUI.color = Color.white;
            GUILayout.Box("", new GUILayoutOption[]{ GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            GUILayout.Label("TRIGGERS", EditorStyles.boldLabel);
            EditorGUILayout.Space();


            // Triggers
            for (int i = 0; i < _triggers.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(_triggers[i].TriggerName + " (" + _triggers[i].AllTriggers.Count() + ")");

                if (GUILayout.Button("Add"))
                {
                    var newGo = new GameObject(_triggers[i].TriggerType.Name);
                    newGo.transform.localScale = new Vector3(5, 5, 5);
                    var trigger = newGo.AddComponent(_triggers[i].TriggerType) as BasePC2D;
                    _triggers[i].AllTriggers.Add(trigger);
                }

                GUI.enabled = _triggers[i].AllTriggers.Count() > 0;

                if (GUILayout.Button(">"))
                {
                    Selection.activeGameObject = ((BasePC2D)_triggers[i].AllTriggers[_triggers[i].TriggerCurrentIndex]).gameObject;
                    SceneView.FrameLastActiveSceneView();
                    EditorGUIUtility.PingObject(((BasePC2D)_triggers[i].AllTriggers[_triggers[i].TriggerCurrentIndex]).gameObject);

                    Selection.activeGameObject = proCamera2D.gameObject;

                    _triggers[i].TriggerCurrentIndex = _triggers[i].TriggerCurrentIndex >= _triggers[i].AllTriggers.Count - 1 ? 0 : _triggers[i].TriggerCurrentIndex + 1;
                }

                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();
            }

            AddSpace();


            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(proCamera2D);
            }
        }

        void AddSpace()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
    }
}