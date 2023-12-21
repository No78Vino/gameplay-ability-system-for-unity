using System;
using System.Collections.Generic;
using GAS.Core;
using GAS.Runtime.Attribute;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.AttributeSet
{
    public class AttributeSetConfigEditorWindow : EditorWindow
    {
        private static List<string> _attributeOptions;

        public string editedName = "";

        public List<string> attributeNames;

        private Action<string, List<string>> _callback;
        private Func<string, bool> _checkAttributeSetValid;

        private List<int> _selectedAttributeIndexs;

        private static List<string> AttributeOptions
        {
            get
            {
                if (_attributeOptions == null)
                {
                    var asset = AssetDatabase.LoadAssetAtPath<AttributeAsset>(GasDefine.GAS_ATTRIBUTE_ASSET_PATH);
                    _attributeOptions = asset?.AttributeNames;
                }

                return _attributeOptions;
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Edit", EditorStyles.boldLabel);

            editedName = EditorGUILayout.TextField("AttributeSet:", editedName);

            EditorGUILayout.Space();

            for (var i = 0; i < attributeNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                _selectedAttributeIndexs[i] = EditorGUILayout.Popup("     Attribute:", _selectedAttributeIndexs[i],
                    AttributeOptions.ToArray());

                // 更新选中的字符串
                attributeNames[i] = _selectedAttributeIndexs[i] < 0
                    ? "ERROR_INVALID_ATTRIBUTE"
                    : AttributeOptions[_selectedAttributeIndexs[i]];

                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    attributeNames.RemoveAt(i);
                    return;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Attribute", GUILayout.Width(100)))
            {
                attributeNames.Add("");
                _selectedAttributeIndexs.Add(-1);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Save")) Save();
        }

        public void Init(string initialString, List<string> attributeNames, Action<string, List<string>> callback,
            Func<string, bool> checkAttributeSetValid)
        {
            editedName = initialString;
            _callback = callback;
            _checkAttributeSetValid = checkAttributeSetValid;
            this.attributeNames = attributeNames;
            _selectedAttributeIndexs = new List<int>();
            foreach (var attributeName in attributeNames)
                _selectedAttributeIndexs.Add(AttributeOptions.IndexOf(attributeName));
        }

        private void Save()
        {
            var valid = _checkAttributeSetValid?.Invoke(editedName) ?? true;
            if (valid)
            {
                _callback?.Invoke(editedName, attributeNames);
                Close();
            }
        }
    }
}