using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DCinematics))]
    [CanEditMultipleObjects]
    public class ProCamera2DCinematicsEditor : Editor
    {
        GUIContent _tooltip;

        MonoScript _script;

        ReorderableList _cinematicTargetsList;

        void OnEnable()
        {
            if (target == null)
                return;
            
            _script = MonoScript.FromMonoBehaviour((ProCamera2DCinematics)target);

            // Cinematic targets list
            _cinematicTargetsList = new ReorderableList(serializedObject, serializedObject.FindProperty("CinematicTargets"), false, true, false, true);

            _cinematicTargetsList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    rect.y += 2;
                    var element = _cinematicTargetsList.serializedProperty.GetArrayElementAtIndex(index);

                    EditorGUI.PrefixLabel(new Rect(
                        rect.x, 
                        rect.y, 
                        90, 
                        10), 
                        new GUIContent("Target", "The target transform"));
                    EditorGUI.PropertyField(new Rect(
                        rect.x + 90,
                        rect.y,
                        90,
                        EditorGUIUtility.singleLineHeight * 1.1f),
                        element.FindPropertyRelative("TargetTransform"), GUIContent.none);

                    EditorGUI.PrefixLabel(new Rect(
                        rect.x + 200, 
                        rect.y, 
                        90, 
                        10), 
                        new GUIContent("EaseType", "The type of the movement the camera will do to reach this target"));
                    EditorGUI.PropertyField(new Rect(
                        rect.x + 260,
                        rect.y,
                        rect.width - 260,
                        EditorGUIUtility.singleLineHeight * 1.1f),
                        element.FindPropertyRelative("EaseType"), GUIContent.none);

                    rect.y += 25;
                    EditorGUI.PrefixLabel(new Rect(
                        rect.x, 
                        rect.y, 
                        90, 
                        10), 
                        new GUIContent("EaseInDuration", "The time it takes to reach this target"));
                    EditorGUI.PropertyField(new Rect(
                        rect.x + 90,
                        rect.y,
                        30,
                        EditorGUIUtility.singleLineHeight * 1.1f),
                        element.FindPropertyRelative("EaseInDuration"), GUIContent.none);
                    
                    EditorGUI.PrefixLabel(new Rect(
                        rect.x + 135, 
                        rect.y, 
                        90, 
                        10), 
                        new GUIContent("HoldDuration", "The time the camera follows this target. If below 0, you'll have to manually move to the next target by using the GoToNextTarget method."));
                    EditorGUI.PropertyField(new Rect(
                        rect.x + 215,
                        rect.y,
                        30,
                        EditorGUIUtility.singleLineHeight * 1.1f),
                        element.FindPropertyRelative("HoldDuration"), GUIContent.none);

                    EditorGUI.PrefixLabel(new Rect(
                        rect.x + 260, 
                        rect.y, 
                        90, 
                        10), 
                        new GUIContent("Zoom", "The zoom the camera will reach when focusing on this target"));
                    EditorGUI.PropertyField(new Rect(
                        rect.x + 300,
                        rect.y,
                        rect.width - 300,
                        EditorGUIUtility.singleLineHeight * 1.1f),
                        element.FindPropertyRelative("Zoom"), GUIContent.none);

                    rect.y += 25;
                    EditorGUI.PrefixLabel(new Rect(
                        rect.x, 
                        rect.y, 
                        90, 
                        10), 
                        new GUIContent("SendMessage Name", "The name of the SendMessage to be sent to the target once it arrives its position"));
                    EditorGUI.PropertyField(new Rect(
                        rect.x + 115,
                        rect.y,
                        70,
                        EditorGUIUtility.singleLineHeight * 1.1f),
                        element.FindPropertyRelative("SendMessageName"), GUIContent.none);

                    EditorGUI.PrefixLabel(new Rect(
                        rect.x + 195, 
                        rect.y, 
                        90, 
                        10), 
                        new GUIContent("SendMessage Param", "The (optional) string parameter to send"));
                    EditorGUI.PropertyField(new Rect(
                        rect.x + 310,
                        rect.y,
                        rect.width - 310,
                        EditorGUIUtility.singleLineHeight * 1.1f),
                        element.FindPropertyRelative("SendMessageParam"), GUIContent.none);
                };

            _cinematicTargetsList.drawHeaderCallback = (Rect rect) =>
                {  
                    EditorGUI.LabelField(rect, "Cinematic Targets");
                };

            _cinematicTargetsList.elementHeight = 90;
            _cinematicTargetsList.draggable = true;
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            
            var proCamera2DCinematics = (ProCamera2DCinematics)target;
            if (proCamera2DCinematics.ProCamera2D == null)
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

            // Targets Drop Area
            EditorGUILayout.Space();
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            var style = new GUIStyle("box");
            if (EditorGUIUtility.isProSkin)
                style.normal.textColor = Color.white;
            GUI.Box(drop_area, "\nDROP CINEMATC TARGETS HERE", style);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (drop_area.Contains(evt.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            foreach (Object dragged_object in DragAndDrop.objectReferences)
                            {
                                var newCinematicTarget = new CinematicTarget
                                {
                                    TargetTransform = ((GameObject)dragged_object).transform,
                                    EaseInDuration = 1f,
                                    HoldDuration = 1f,
                                    EaseType = EaseType.EaseOut
                                };

                                proCamera2DCinematics.CinematicTargets.Add(newCinematicTarget);
                                EditorUtility.SetDirty(proCamera2DCinematics);
                            }
                        }
                    }
                    break;
            }

            EditorGUILayout.Space();

            // Remove empty targets
            for (int i = 0; i < proCamera2DCinematics.CinematicTargets.Count; i++)
            {
                if (proCamera2DCinematics.CinematicTargets[i].TargetTransform == null)
                {
                    proCamera2DCinematics.CinematicTargets.RemoveAt(i);
                }
            }

            // Camera targets list
            _cinematicTargetsList.DoLayoutList();
            EditorGUILayout.Space();

            // End duration
            _tooltip = new GUIContent("End Duration", "How long it takes for the camera to get back to its regular targets");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EndDuration"), _tooltip);

            // End ease type
            _tooltip = new GUIContent("End Ease Type", "The ease type of the end animation");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("EndEaseType"), _tooltip);
            
            // Use numeric boundaries
            _tooltip = new GUIContent("Use Numeric Boundaries", "If existent, the camera movement will be limited by the numeric boundaries");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseNumericBoundaries"), _tooltip);

            // Letterbox
            _tooltip = new GUIContent("Use Letterbox", "If checked, the camera will show black bars on top and bottom during the cinematic sequence");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseLetterbox"), _tooltip);

            if (proCamera2DCinematics.UseLetterbox)
            {
                // Letterbox amount
                _tooltip = new GUIContent("Letterbox Amount", "");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("LetterboxAmount"), _tooltip);

                // Letterbox animation duration
                _tooltip = new GUIContent("Letterbox Anim Duration", "");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("LetterboxAnimDuration"), _tooltip);

                // Letterbox color
                _tooltip = new GUIContent("Letterbox Color", "");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("LetterboxColor"), _tooltip);
            }

            EditorGUILayout.Space();

            // Events
            // Cinematic started event
            _tooltip = new GUIContent("OnCinematicStarted", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnCinematicStarted"), _tooltip);

            // Cinematic target reached event
            _tooltip = new GUIContent("OnCinematicTargetReached", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnCinematicTargetReached"), _tooltip);

            // Cinematic finished event
            _tooltip = new GUIContent("OnCinematicFinished", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnCinematicFinished"), _tooltip);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Test buttons
            GUI.enabled = Application.isPlaying && proCamera2DCinematics.CinematicTargets.Count > 0;
            if (GUILayout.Button((proCamera2DCinematics.IsPlaying ? "Stop" : "Start") + " Cinematic"))
            {
                if (proCamera2DCinematics.IsPlaying)
                    proCamera2DCinematics.Stop();
                else
                    proCamera2DCinematics.Play();
            }
            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
        }
    }
}