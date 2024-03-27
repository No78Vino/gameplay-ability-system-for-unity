using GAS.Editor.Validation;

#if UNITY_EDITOR
namespace GAS.Editor.General
{
    using UnityEditor;
    using UnityEngine;

    public class StringEditWindow : EditorWindow
    {
        private string _tip;
        private string _editedString = "";
        private ValidationDelegate _validator;
        private System.Action<string> _callback;

        public static void OpenWindow(string tip, string initialString, ValidationDelegate validator,
            System.Action<string> callback, string title = "Input")
        {
            var window = GetWindow<StringEditWindow>();
            window.Init(tip, initialString, validator, callback);
            window.titleContent = new GUIContent(title);
            window.Show();
        }

        public static void OpenWindow(string tip, string initialString, System.Action<string> callback,
            string title = "Input")
        {
            OpenWindow(tip, initialString, null, callback, title);
        }

        private void OnGUI()
        {
            _editedString = EditorGUILayout.TextField($"{_tip}:", _editedString);

            EditorGUILayout.Space();

            if (GUILayout.Button("Save"))
            {
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
        }

        private void Init(string tip, string initialString, ValidationDelegate validator,
            System.Action<string> callback)
        {
            _tip = tip;
            _editedString = initialString;
            _validator = validator;
            _callback = callback;
        }
    }
}
#endif