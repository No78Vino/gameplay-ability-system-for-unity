using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DShake))]
    public class ProCamera2DShakeEditor : Editor
    {
        MonoScript _script;
        GUIContent _tooltip;

        ReorderableList _shakePresetsList;
        ReorderableList _constantShakePresetsList;

        int _currentPickerWindow;

        double _toggleButtonClick = 0.0;

        void OnEnable()
        {
            if (target == null)
                return;

            var proCamera2DShake = (ProCamera2DShake)target;

            _script = MonoScript.FromMonoBehaviour(proCamera2DShake);

            //
            // ShakePresets list
            //
            _shakePresetsList = new ReorderableList(serializedObject, serializedObject.FindProperty("ShakePresets"), true, true, true, true);

            _shakePresetsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                var element = _shakePresetsList.serializedProperty.GetArrayElementAtIndex(index);

                if (element == null || element.objectReferenceValue == null)
                {
                    proCamera2DShake.ShakePresets.RemoveAt(index);
                    return;
                }

                // Name field
                EditorGUI.LabelField(
                    new Rect(
                        rect.x,
                        rect.y,
                        rect.width / 2f,
                        EditorGUIUtility.singleLineHeight * 1.1f),
                    ((ShakePreset)element.objectReferenceValue).name);

                // Edit button
                if (GUI.Button(new Rect(
                            rect.x + 2 * rect.width / 4f + 5,
                            rect.y - 2,
                            rect.width / 4f - 5,
                            EditorGUIUtility.singleLineHeight * 1.1f), "Edit"))
                {
                    Selection.activeObject = (ShakePreset)element.objectReferenceValue;
                }

                // Shake button
                GUI.enabled = Application.isPlaying;
                if (GUI.Button(new Rect(
                            rect.x + 3 * rect.width / 4f + 5,
                            rect.y - 2,
                            rect.width / 4f - 5,
                            EditorGUIUtility.singleLineHeight * 1.1f), "Shake!"))
                {
                    proCamera2DShake.Shake((ShakePreset)element.objectReferenceValue);
                }
                GUI.enabled = true;

            };

            _shakePresetsList.onAddCallback = (list) =>
            {
                _currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive) + 100;
                EditorGUIUtility.ShowObjectPicker<ShakePreset>(null, false, "", _currentPickerWindow);
            };

            _shakePresetsList.onSelectCallback = (list) =>
            {
                var element = _shakePresetsList.serializedProperty.GetArrayElementAtIndex(list.index);

                if (element != null)
                    EditorGUIUtility.PingObject(element.objectReferenceValue);
            };

            _shakePresetsList.onRemoveCallback = (list) =>
            {
                Undo.RecordObject(proCamera2DShake, "Removed shake preset");
                var element = _shakePresetsList.serializedProperty.GetArrayElementAtIndex(list.index);

                proCamera2DShake.ShakePresets.Remove((ShakePreset)element.objectReferenceValue);
            };

            _shakePresetsList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Shake Presets");
            };


            //
            // ConstantShakePresets list
            //

            _constantShakePresetsList = new ReorderableList(serializedObject, serializedObject.FindProperty("ConstantShakePresets"), true, true, true, true);

            _constantShakePresetsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                var element = _constantShakePresetsList.serializedProperty.GetArrayElementAtIndex(index);

                if (element == null || element.objectReferenceValue == null)
                {
                    proCamera2DShake.ConstantShakePresets.RemoveAt(index);
                    return;
                }

                var preset = (ConstantShakePreset)element.objectReferenceValue;

                // Name field
                EditorGUI.LabelField(
                    new Rect(
                        rect.x,
                        rect.y,
                        rect.width / 2f,
                        EditorGUIUtility.singleLineHeight * 1.1f),
                    (preset).name);

                // Toggle to enable on start
                if (!Application.isPlaying)
                {
                    if(GUI.Toggle(new Rect(
                            rect.x + 1 * rect.width / 4f + 5,
                            rect.y - 2,
                            rect.width / 4f - 5,
                            EditorGUIUtility.singleLineHeight * 1.1f),
                                  proCamera2DShake.StartConstantShakePreset == preset,
                                  "Enable on Start"))
                    {
                        if (Event.current.type == EventType.Used && EditorApplication.timeSinceStartup - _toggleButtonClick > .5)
                        {
                            if (proCamera2DShake.StartConstantShakePreset == preset)
                                proCamera2DShake.StartConstantShakePreset = null;
                            else
                                proCamera2DShake.StartConstantShakePreset = preset;

                            _toggleButtonClick = EditorApplication.timeSinceStartup;
                        }

                    }
                }

                // Edit button
                if (GUI.Button(new Rect(
                            rect.x + 2 * rect.width / 4f + 5,
                            rect.y - 2,
                            rect.width / 4f - 5,
                            EditorGUIUtility.singleLineHeight * 1.1f), "Edit"))
                {
                    Selection.activeObject = preset;
                }

                // Shake button
                GUI.enabled = Application.isPlaying;
                if (GUI.Button(new Rect(
                            rect.x + 3 * rect.width / 4f + 5,
                            rect.y - 2,
                            rect.width / 4f - 5,
                    EditorGUIUtility.singleLineHeight * 1.1f), (proCamera2DShake.CurrentConstantShakePreset != null && proCamera2DShake.CurrentConstantShakePreset.GetInstanceID() == preset.GetInstanceID()) ? "Disable" : "Enable"))
                {
                    if (proCamera2DShake.CurrentConstantShakePreset == null || proCamera2DShake.CurrentConstantShakePreset.GetInstanceID() != preset.GetInstanceID())
                    {
                        proCamera2DShake.ConstantShake(preset);
                    }
                    else
                    {
                        proCamera2DShake.StopConstantShaking();
                    }
                }
                GUI.enabled = true;
            };

            _constantShakePresetsList.onAddCallback = (list) =>
            {
                _currentPickerWindow = GUIUtility.GetControlID(FocusType.Passive) + 100;
                EditorGUIUtility.ShowObjectPicker<ConstantShakePreset>(null, false, "", _currentPickerWindow);
            };

            _constantShakePresetsList.onSelectCallback = (list) =>
            {
                var element = _constantShakePresetsList.serializedProperty.GetArrayElementAtIndex(list.index);

                if (element != null)
                    EditorGUIUtility.PingObject(element.objectReferenceValue);
            };

            _constantShakePresetsList.onRemoveCallback = (list) =>
            {
                Undo.RecordObject(proCamera2DShake, "Removed shake preset");
                var element = _constantShakePresetsList.serializedProperty.GetArrayElementAtIndex(list.index);

                proCamera2DShake.ConstantShakePresets.Remove((ConstantShakePreset)element.objectReferenceValue);
            };

            _constantShakePresetsList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Constant Shake Presets");
            };
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
                return;
            
            EditorGUI.BeginChangeCheck();
            
            var proCamera2DShake = (ProCamera2DShake)target;

            if (proCamera2DShake.ProCamera2D == null)
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
            EditorGUILayout.Space();

            // Create ShakePreset button
            if (GUILayout.Button("Create ShakePreset"))
            {
                Undo.RecordObject(proCamera2DShake, "Added shake preset");

                proCamera2DShake.ShakePresets.Add(ScriptableObjectUtility.CreateAsset<ShakePreset>("ShakePreset"));
            }

            // Shake Presets list
            EditorGUILayout.Space();
            _shakePresetsList.DoLayoutList();

            // ShakePreset selected from picker window
            if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == _currentPickerWindow)
            {
                var preset = EditorGUIUtility.GetObjectPickerObject() as ShakePreset;

                if (preset != null)
                {
                    if (!proCamera2DShake.ShakePresets.Contains(preset))
                    {
                        Undo.RecordObject(proCamera2DShake, "Added shake preset");

                        proCamera2DShake.ShakePresets.Add(preset);
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Create ConstantShakePreset button
            if (GUILayout.Button("Create ConstantShakePreset"))
            {
                Undo.RecordObject(proCamera2DShake, "Added shake preset");

                ScriptableObjectUtility.CreateAsset<ConstantShakePreset>("ConstantShakePreset");
            }

            // ConstantShake Presets list
            EditorGUILayout.Space();
            _constantShakePresetsList.DoLayoutList();

            // ConstantShakePreset selected from picker window
            if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == _currentPickerWindow)
            {
                var preset = EditorGUIUtility.GetObjectPickerObject() as ConstantShakePreset;

                if (preset != null)
                {
                    if (!proCamera2DShake.ConstantShakePresets.Contains(preset))
                    {
                        Undo.RecordObject(proCamera2DShake, "Added shake preset");

                        proCamera2DShake.ConstantShakePresets.Add(preset);
                    }
                }
            }

            // Save changes
            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(proCamera2DShake);
            }
        }
    }
}