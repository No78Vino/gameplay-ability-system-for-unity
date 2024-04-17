#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GAS.Editor.Validation;
    using UnityEditor;
    using UnityEngine;
    
    public sealed class AttributeEditorWindow : EditorWindow
    {
        public sealed class Data
        {
            public string Name;
            public string Comment;

            public Data(string name = null, string comment = null)
            {
                Name = name ?? "";
                Comment = comment ?? "He is very lazy and left nothing behind.";
            }

            public override string ToString()
            {
                return $"Name: {Name}, Comment: {Comment}";
            }
        }

        Data _oldData;
        Data _newData;
        private IEnumerable<string> _nameBlackList;
        private Action<Data> _callback;

        public static void OpenWindow(Data data, IEnumerable<string> nameBlackList,
            Action<Data> callback, string title = "AttributeEditorWindow")
        {
            var window = GetWindow<AttributeEditorWindow>();
            window.Init(data, nameBlackList, callback);
            window.titleContent = new GUIContent(title);
            window.ShowModalUtility();
        }

        private void Init(Data data, IEnumerable<string> nameBlackList,
            Action<Data> callback)
        {
            _oldData = data;
            _newData = new Data(data.Name, data.Comment);
            _nameBlackList = nameBlackList;
            _callback = callback;
        }


        private void OnGUI()
        {
            _newData.Name = EditorGUILayout.TextField($"Attribute Name:", _newData.Name);
            _newData.Comment = EditorGUILayout.TextField($"Comment:", _newData.Comment);

            EditorGUILayout.Space();

            if (GUILayout.Button("Ok"))
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
                if (EditorUtility.DisplayDialog("Warning", "Are you sure to cancel?", "Yes", "No"))
                {
                    Close();
                }
            }
        }

        private void Save()
        {
            // name is empty means create a new attribute
            // if name is not empty, show the warning dialog
            if (_oldData.Name != "" && (_oldData.Name != _newData.Name || _oldData.Comment != _newData.Comment))
            {
                var sb = new StringBuilder();
                if (_oldData.Name != _newData.Name)
                {
                    sb.AppendLine($"- name: \"{_oldData.Name}\" => \"{_newData.Name}\"");
                }

                if (_oldData.Comment != _newData.Comment)
                {
                    sb.AppendLine($"- comment: \"{_oldData.Comment}\" => \"{_newData.Comment}\"");
                }

                if (!EditorUtility.DisplayDialog("The following changes will be made:", sb.ToString(), "Yes", "No"))
                {
                    return;
                }
            }

            var validationResult = Validations.ValidateVariableName(_newData.Name);
            if (!validationResult.IsValid)
            {
                EditorUtility.DisplayDialog("Name error", validationResult.Message, "OK");
                return;
            }

            if (_nameBlackList.Contains(_newData.Name))
            {
                EditorUtility.DisplayDialog("Name error", "The name already exists!", "OK");
                return;
            }

            _callback?.Invoke(_newData);
            Close();
        }
    }
}
#endif