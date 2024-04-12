using GAS.Editor.Validation;

#if UNITY_EDITOR
namespace GAS.Editor.General
{
    using UnityEditor;
    using UnityEngine;

    public class StringEditWindow : EditorWindow
    {
        private string _tip;
        private string _initialString;
        private string _editedString = "";
        private ValidationDelegate _validator;
        private System.Action<string> _callback;

        private bool focusInputField = false;

        public static void OpenWindow(string tip, string initialString, ValidationDelegate validator,
            System.Action<string> callback, string title = "Input")
        {
            var window = GetWindow<StringEditWindow>();
            window.Init(tip, initialString, validator, callback);
            window.titleContent = new GUIContent(title);
            window.ShowModalUtility();
        }

        public static void OpenWindow(string tip, string initialString, System.Action<string> callback,
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

            EditorGUILayout.HelpBox("Press Enter to save, Esc to cancel", MessageType.None);

            if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyUp)
            {
                Event.current.Use();
                Save();
            }

            if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyUp)
            {
                Event.current.Use();
                Close();
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
            System.Action<string> callback)
        {
            _tip = tip;
            _initialString = initialString;
            _editedString = initialString;
            _validator = validator;
            _callback = callback;
        }
    }
}
#endif