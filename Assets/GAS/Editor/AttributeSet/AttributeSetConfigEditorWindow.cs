using System.Linq;

#if UNITY_EDITOR
namespace GAS.Editor
{
    using System;
    using System.Collections.Generic;
    using GAS;
    using Editor;
    using Runtime;
    using UnityEditor;
    using UnityEngine;

    public class AttributeSetConfigEditorWindow : EditorWindow
    {
        private static List<string> _attributeOptions;

        public string editedName = "";

        public List<string> attributeNames;

        private Action<string, List<string>> _callback;
        private Func<AttributeSetConfig, bool> _checkAttributeSetValid;

        private List<int> _selectedAttributeIndexes;

        private GUIStyle BigFontLabelStyle;

        private static List<string> AttributeOptions
        {
            get
            {
                if (_attributeOptions == null)
                {
                    var asset = AttributeAsset.LoadOrCreate();
                    _attributeOptions = asset?.AttributeNames?.OrderBy(x => x).ToList();
                }

                return _attributeOptions;
            }
        }

        public static void OpenWindow(string initialString, List<string> attributeNames,
            Action<string, List<string>> callback, Func<AttributeSetConfig, bool> checkAttributeSetValid)
        {
            var window = GetWindow<AttributeSetConfigEditorWindow>();
            window.Init(initialString, attributeNames, callback, checkAttributeSetValid);
            window.Show();
        }

        Vector2 scrollPosition = Vector2.zero;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AttributeSet:", GUILayout.Width(80));
            editedName = EditorGUILayout.TextField("", editedName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Attribute", GUILayout.Width(100)))
            {
                attributeNames.Add("");
                _selectedAttributeIndexes.Add(-1);
            }

            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUI.skin.box);

            for (var i = 0; i < attributeNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Attribute:", GUILayout.Width(80));
                _selectedAttributeIndexes[i] = EditorGUILayout.Popup("", _selectedAttributeIndexes[i],
                    AttributeOptions.ToArray());

                // 更新选中的字符串
                attributeNames[i] = _selectedAttributeIndexes[i] < 0
                    ? ""
                    : AttributeOptions[_selectedAttributeIndexes[i]];

                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    attributeNames.RemoveAt(i);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save")) Save();
        }

        public void Init(string initialString, List<string> attributeNames, Action<string, List<string>> callback,
            Func<AttributeSetConfig, bool> checkAttributeSetValid)
        {
            editedName = initialString;
            _callback = callback;
            _checkAttributeSetValid = checkAttributeSetValid;
            this.attributeNames = attributeNames;
            _selectedAttributeIndexes = new List<int>();
            foreach (var attributeName in attributeNames)
                _selectedAttributeIndexes.Add(AttributeOptions.IndexOf(attributeName));


            BigFontLabelStyle = new GUIStyle(EditorStyles.label);
            BigFontLabelStyle.fontSize = 20; // 设置字体大小为 16
        }

        private void Save()
        {
            AttributeSetConfig attributeSetConfig = new AttributeSetConfig()
            {
                Name = editedName,
                AttributeNames = attributeNames
            };
            var valid = _checkAttributeSetValid?.Invoke(attributeSetConfig) ?? true;
            if (valid)
            {
                _callback?.Invoke(editedName, attributeNames);
                Close();
            }
        }

        private void OnDisable()
        {
            Save();
        }
    }
}
#endif