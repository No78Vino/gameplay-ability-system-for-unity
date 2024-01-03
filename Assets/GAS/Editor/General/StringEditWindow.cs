using System;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.General
{
    public class StringEditWindow : EditorWindow
    {
        private Action<string> callback;
        private string editedString = "";
        private string tip;
        public static void OpenWindow(string initialString, Action<string> callback,string tip)
        {
            var window = GetWindow<StringEditWindow>();
            window.Init(initialString, callback, tip);
            window.titleContent = new GUIContent("Input");
            window.Show();
        }
        private void OnGUI()
        {
            // Display the input field to edit the string
            //editedString = EditorGUILayout.TextField("Attribute:", editedString);
            editedString = EditorGUILayout.TextField($"{tip}:", editedString);

            EditorGUILayout.Space();

            // Save button to apply changes
            if (GUILayout.Button("Save"))
            {
                callback?.Invoke(editedString);
                Close();
            }
        }

        private void Init(string initialString, Action<string> callback,string tip)
        {
            editedString = initialString;
            this.callback = callback;
            this.tip = tip;
        }
    }
}