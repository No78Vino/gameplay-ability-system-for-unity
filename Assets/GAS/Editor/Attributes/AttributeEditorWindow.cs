using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GAS.Editor.Validation;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
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
        }

        Data _data;
        private IEnumerable<string> _nameBlackList;
        private Action<Data> _callback;

        public static void OpenWindow(Data data, IEnumerable<string> nameBlackList,
            Action<Data> callback, string title = "AttributeEditorWindow")
        {
            var window = GetWindow<AttributeEditorWindow>();
            window.Init(data, nameBlackList, callback);
            window.titleContent = new GUIContent(title);
            window.Show();
        }

        private void Init(Data data, IEnumerable<string> nameBlackList,
            Action<Data> callback)
        {
            _data = data;
            _nameBlackList = nameBlackList;
            _callback = callback;
        }


        private void OnGUI()
        {
            _data.Name = EditorGUILayout.TextField($"Attribute Name:", _data.Name);
            _data.Comment = EditorGUILayout.TextField($"Comment:", _data.Comment);

            EditorGUILayout.Space();


            if (GUILayout.Button("Ok"))
            {
                var validationResult = Validations.ValidateVariableName(_data.Name);
                if (!validationResult.IsValid)
                {
                    EditorUtility.DisplayDialog("Name error", validationResult.Message, "OK");
                    return;
                }

                if (_nameBlackList.Contains(_data.Name))
                {
                    EditorUtility.DisplayDialog("Name error", "The name already exists!", "OK");
                    return;
                }

                _callback?.Invoke(_data);
                Close();
            }
        }
    }
}