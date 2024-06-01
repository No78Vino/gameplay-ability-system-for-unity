using System;
using GAS.General.Validation;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.General
{
    public class StringEditWindow : EditorWindow
    {
        private string _tip;
        private string _initialString;
        private string _editedString = "";
        private ValidationDelegate _validator;
        private Action<string> _callback;

        private bool focusInputField = false;

        public static void OpenWindow(string tip, string initialString, ValidationDelegate validator,
            Action<string> callback, string title = "Input")
        {
            var window = GetWindow<StringEditWindow>();
            window.Init(tip, initialString, validator, callback);
            window.titleContent = new GUIContent(title);
            window.ShowModalUtility();
        }

        public static void OpenWindow(string tip, string initialString, Action<string> callback,
            string title = "Input")
        {
            OpenWindow(tip, initialString, null, callback, title);
        }

        private void OnGUI()
        {
            const string ctrlName = "InputField";
            if (!focusInputField)
            {
                GUI.SetNextControlName(ctrlName);
            }

            _editedString = EditorGUILayout.TextField($"{_tip}:", _editedString);

            // Focus the input field
            if (!focusInputField)
            {
                focusInputField = true;
                EditorGUI.FocusTextInControl(ctrlName);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save"))
            {
                Save();
            }
        }

        private void Save()
        {
            if (!string.IsNullOrWhiteSpace(_initialString) && (_initialString != _editedString))
            {
                if (!EditorUtility.DisplayDialog("Warning",
                        $"Are you sure to save the changes?\n\n    {_initialString} => {_editedString}", "Yes", "No"))
                {
                    return;
                }
            }

            if (_validator != null)
            {
                var validationResult = _validator.Invoke(_editedString);
                if (!validationResult.IsValid)
                {
                    EditorUtility.DisplayDialog("Validation Failed", validationResult.Message, "OK");
                    return;
                }
            }

            _callback?.Invoke(_editedString);
            Close();
        }

        private void Init(string tip, string initialString, ValidationDelegate validator,
            Action<string> callback)
        {
            _tip = tip;
            _initialString = initialString;
            _editedString = initialString;
            _validator = validator;
            _callback = callback;
        }
    }
}