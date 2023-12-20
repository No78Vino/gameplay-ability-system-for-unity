using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _Test
{
    public class StringListEditorWindow : EditorWindow
    {
        private int selectedStringIndex = -1;
        private readonly List<string> stringList = new() { "String 1", "String 2", "String 3" };

        private void OnGUI()
        {
            GUILayout.Label("String List Editor", EditorStyles.boldLabel);

            // Display the list of strings
            for (var i = 0; i < stringList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // Show the string
                EditorGUILayout.LabelField($"String {i + 1}: {stringList[i]}");

                // Edit button to modify the selected string
                if (GUILayout.Button("Edit", GUILayout.Width(50)))
                {
                    selectedStringIndex = i;
                    OpenEditStringPopup();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        [MenuItem("Test/String List Editor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(StringListEditorWindow));
        }

        private void OpenEditStringPopup()
        {
            if (selectedStringIndex >= 0 && selectedStringIndex < stringList.Count)
            {
                // Open a custom editor window to edit the selected string
                var window = CreateInstance<StringEditWindow>();
                window.Init(stringList[selectedStringIndex], UpdateString);
                window.ShowUtility();
            }
        }

        private void UpdateString(string updatedString)
        {
            if (selectedStringIndex >= 0 && selectedStringIndex < stringList.Count)
                stringList[selectedStringIndex] = updatedString;
        }
    }

// Custom editor window to edit strings
    public class StringEditWindow : EditorWindow
    {
        private Action<string> callback;
        private string editedString = "";

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Edit String", EditorStyles.boldLabel);

            // Display the input field to edit the string
            editedString = EditorGUILayout.TextField("String Value:", editedString);

            EditorGUILayout.Space();

            // Save button to apply changes
            if (GUILayout.Button("Save"))
            {
                callback?.Invoke(editedString);
                Close();
            }
        }

        public void Init(string initialString, Action<string> callback)
        {
            editedString = initialString;
            this.callback = callback;
        }
    }
}