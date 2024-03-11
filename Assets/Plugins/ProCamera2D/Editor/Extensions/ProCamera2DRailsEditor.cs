using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditorInternal;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DRails))]
    public class ProCamera2DRailsEditor : Editor
    {
        Func<Vector3, float> Vector3H;
        Func<Vector3, float> Vector3V;
        Func<float, float, Vector3> VectorHV;

        static List<Vector3> _playModeNodes = new List<Vector3>();
        static string _currentScene;
        
        MonoScript _script;
        GUIContent _tooltip;
        
        ReorderableList _targetsList;

        void OnEnable()
        {
            if (target == null)
                return;

            var proCamera2DRails = (ProCamera2DRails)target;

            // Get nodes from play mode
            serializedObject.Update();

            if (_currentScene != UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name)
                _playModeNodes.Clear();

            _currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;

            if (!Application.isPlaying && _playModeNodes.Count > 0)
            {
                var list = serializedObject.FindProperty("RailNodes");
                list.ClearArray();
                for (int i = 0; i < _playModeNodes.Count; i++)
                {
                    list.InsertArrayElementAtIndex(i);
                    var preset = list.GetArrayElementAtIndex(i);
                    preset.vector3Value = _playModeNodes[i];
                }
                _playModeNodes.Clear();
            }
            serializedObject.ApplyModifiedProperties();
            
            // Script
            _script = MonoScript.FromMonoBehaviour(proCamera2DRails);

            // Create Vector conversion methods
            switch (proCamera2DRails.ProCamera2D.Axis)
            {
                case MovementAxis.XY:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.y;
                    VectorHV = (h, v) => new Vector3(h, v, 0);
                    break;
                case MovementAxis.XZ:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.z;
                    VectorHV = (h, v) => new Vector3(h, 0, v);
                    break;
                case MovementAxis.YZ:
                    Vector3H = vector => vector.z;
                    Vector3V = vector => vector.y;
                    VectorHV = (h, v) => new Vector3(0, v, h);
                    break;
            }
            
            // Create nodes if non-existant
            if (proCamera2DRails.RailNodes.Count < 2)
            {
                proCamera2DRails.RailNodes.Add(VectorHV(1, 1));
                proCamera2DRails.RailNodes.Add(VectorHV(2, 2));
            }
            
            // Show correct axis name
            var hAxis = "";
            var vAxis = "";
            switch (proCamera2DRails.ProCamera2D.Axis)
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
        }

        void OnDisable()
        {
            var proCamera2DRails = (ProCamera2DRails)target;

            _playModeNodes = proCamera2DRails.RailNodes;
        }
        
        public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            
            var proCamera2DRails = (ProCamera2DRails)target;
            
            if(proCamera2DRails.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);
                
            serializedObject.Update();
            
            // Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pc2D"), _tooltip);
            EditorGUILayout.Space();
            
            // Targets Drop Area
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            var style = new GUIStyle("box");
            if (EditorGUIUtility.isProSkin)
                style.normal.textColor = Color.white;
            GUI.Box(drop_area, "\nDROP CAMERA TARGETS HERE", style);

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

                        foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences)
                        {
                            var newCameraTarget = new CameraTarget
                            {
                                TargetTransform = ((GameObject)dragged_object).transform,
                                TargetInfluence = 1f
                            };
							
                            proCamera2DRails.CameraTargets.Add(newCameraTarget);
                            EditorUtility.SetDirty(proCamera2DRails);
                        }
                    }
                    break;
            }
            EditorGUILayout.Space();
            
            // Targets List
            _targetsList.DoLayoutList();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            // Follow mode
            _tooltip = new GUIContent("FollowMode", "This defines which axis to use to calculate the targets position on the rails");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FollowMode"), _tooltip);
            EditorGUILayout.Space();
            
            // Apply modified properties
            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            var proCamera2DRails = (ProCamera2DRails)target;

            // Return if less than 2 nodes
            var nodesCount = proCamera2DRails.RailNodes.Count;
            if (nodesCount < 2)
                return;

            // Detect delete mode
            bool deleteMode = false;
            Event e = Event.current;
            if (e.alt || e.shift)
            {
                deleteMode = true;
            }

            // Handles color
            Handles.color = EditorPrefsX.GetColor(PrefsData.RailsColorKey, PrefsData.RailsColorValue);

            // Draw line between nodes
            Handles.DrawPolyLine(proCamera2DRails.RailNodes.ToArray());

            // Handle size
            float handleSize = HandleUtility.GetHandleSize(Vector3.zero) * .1f;

            // Draw left and right handles
            var rightHandlePos = (proCamera2DRails.RailNodes[nodesCount - 1] - proCamera2DRails.RailNodes[nodesCount - 2]).normalized * .5f + proCamera2DRails.RailNodes[nodesCount - 1];
            if (Handles.Button(
                rightHandlePos, 
                Quaternion.identity, 
                handleSize * .5f, 
                handleSize, 
                Handles.RectangleHandleCap))
            {
                Undo.RecordObject(proCamera2DRails, "Add");
                proCamera2DRails.RailNodes.Insert(nodesCount, rightHandlePos);
            }

            var leftHandlesPos = (proCamera2DRails.RailNodes[0] - proCamera2DRails.RailNodes[1]).normalized * .5f + proCamera2DRails.RailNodes[0];
            if (Handles.Button(
                leftHandlesPos, 
                Quaternion.identity, 
                handleSize * .5f, 
                handleSize, 
                Handles.RectangleHandleCap))
            {
                Undo.RecordObject(proCamera2DRails, "Add");
                proCamera2DRails.RailNodes.Insert(0, leftHandlesPos);
            }

            // Snap value
            var pointSnap = Vector3.one * EditorPrefs.GetFloat("RailsSnapping", PrefsData.RailsSnapping);

            // Draw a handle for each node
            for (int i = 0; i < nodesCount; i++)
            {
                var oldPos = proCamera2DRails.RailNodes[i];
#if UNITY_2022_1_OR_NEWER
                var newPos = Handles.FreeMoveHandle(oldPos, handleSize, pointSnap, Handles.DotHandleCap);
#else
                var newPos = Handles.FreeMoveHandle(oldPos, Quaternion.identity, handleSize, pointSnap, Handles.DotHandleCap);
#endif
                
                // Move
                if (newPos != oldPos)
                {
                    newPos.x = Handles.SnapValue(newPos.x, pointSnap.x);
                    newPos.y = Handles.SnapValue(newPos.y, pointSnap.y);
                    newPos.z = Handles.SnapValue(newPos.z, pointSnap.z);

                    Undo.RecordObject(proCamera2DRails, "Move");
                    proCamera2DRails.RailNodes[i] = VectorHV(Vector3H(newPos), Vector3V(newPos));
                }

                // Draw the midpoint button
                if (i > 0)
                {
                    var midPoint = Vector3.Lerp(proCamera2DRails.RailNodes[i - 1], proCamera2DRails.RailNodes[i], 0.5f);

                    if (Handles.Button(
                        midPoint, 
                        Quaternion.identity, 
                        handleSize * .5f, 
                        handleSize, 
                        Handles.RectangleHandleCap))
                    {
                        Undo.RecordObject(proCamera2DRails, "Add");
                        proCamera2DRails.RailNodes.Insert(i, midPoint);
                    }
                }

                // Draw the delete button
                if (deleteMode && nodesCount > 2)
                {
                    Handles.color = Color.red;
                    var deleteButtonPos = proCamera2DRails.RailNodes[i];
                    if (Handles.Button(
                        deleteButtonPos, 
                        Quaternion.identity, 
                        handleSize, 
                        handleSize, 
                        Handles.DotHandleCap))
                    {
                        Undo.RecordObject(proCamera2DRails, "Remove");
                        proCamera2DRails.RailNodes.RemoveAt(i);
                        return;
                    }
                    Handles.color = EditorPrefsX.GetColor(PrefsData.RailsColorKey, PrefsData.RailsColorValue);
                }
            }
        }
    }
}