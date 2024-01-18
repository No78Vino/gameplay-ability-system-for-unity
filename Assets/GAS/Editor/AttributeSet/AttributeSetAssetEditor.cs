#if UNITY_EDITOR
namespace GAS.Editor.AttributeSet
{
    using System.Collections.Generic;
    using System.Linq;
    using GAS.Runtime.AttributeSet;
    using UnityEditor;
    using UnityEngine;
    
    //[CustomEditor(typeof(AttributeSetAsset))]
    public class AttributeSetAssetEditor : UnityEditor.Editor
    {
        private int _selectedIndex = -1;
        private GUIStyle BigFontLabelStyle;
        private AttributeSetAsset Asset => (AttributeSetAsset)target;

        private void Awake()
        {
            BigFontLabelStyle = new GUIStyle(EditorStyles.label);
            BigFontLabelStyle.fontSize = 16;
            BigFontLabelStyle.fontStyle = FontStyle.Bold;
        }

        public override void OnInspectorGUI()
        {
            ToolBar();
            GUILayout.Space(3f);

            for (var i = 0; i < Asset.AttributeSetConfigs.Count; i++)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"No.{i} : {Asset.AttributeSetConfigs[i].Name}", BigFontLabelStyle);

                if (GUILayout.Button("Edit", GUILayout.Width(50)))
                {
                    _selectedIndex = i;
                    OpenEditPopup();
                }

                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    _selectedIndex = i;
                    Remove(_selectedIndex);
                    return;
                }

                EditorGUILayout.EndHorizontal();

                foreach (var t in Asset.AttributeSetConfigs[i].AttributeNames)
                    EditorGUILayout.LabelField($"       ||--> {t}");

                EditorGUILayout.EndVertical();
            }
        }

        private void ToolBar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var style = "miniButton";
                if (GUILayout.Button("Add", style)) Add();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Save", style)) Save();

                if (GUILayout.Button("Gen AttributeCollection Code", style)) GenCode();
            }
        }

        private void Add()
        {
            AttributeSetConfigEditorWindow.OpenWindow("", new List<string>(), UpdateAttribute, CheckAttributeSetValid);
        }

        private void Remove(int index)
        {
            var attributeSetName = Asset.AttributeSetConfigs[index].Name;
            var result = EditorUtility.DisplayDialog("Confirmation",
                $"Are you sure you want to REMOVE AttributeSet:{attributeSetName}?",
                "Yes", "No");

            if (!result) return;
            Asset.AttributeSetConfigs.RemoveAt(index);
            Save();
        }

        private void Save()
        {
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
        }

        private void GenCode()
        {
            Save();
            AttributeSetClassGen.Gen();
            AssetDatabase.Refresh();
        }

        private bool CheckAttributeSetValid(AttributeSetConfig attributeSet)
        {
            // Check AttributeSet name
            if (Asset.AttributeSetConfigs.Any(a => a.Name == attributeSet.Name))
            {
                EditorUtility.DisplayDialog("Warning", "AttributeSet name already exists!", "OK");
                return false;
            }

            if (!EditorUtil.IsValidClassName(attributeSet.Name))
            {
                EditorUtility.DisplayDialog("Warning", "Invalid AttributeSet name!Please check the naming rules.",
                    "OK");
                return false;
            }

            // Check Attributes
            if (attributeSet.AttributeNames.Count == 0)
            {
                EditorUtility.DisplayDialog("Warning", "AttributeSet must have at least one attribute!", "OK");
                return false;
            }

            if (attributeSet.AttributeNames.Any(string.IsNullOrEmpty))
            {
                EditorUtility.DisplayDialog("Warning", "Attribute name cannot be empty!", "OK");
                return false;
            }

            return true;
        }

        private void OpenEditPopup()
        {
            if (_selectedIndex < 0 || _selectedIndex >= Asset.AttributeSetConfigs.Count) return;

            var setName = Asset.AttributeSetConfigs[_selectedIndex].Name;
            var attributeNames = Asset.AttributeSetConfigs[_selectedIndex].AttributeNames;
            AttributeSetConfigEditorWindow.OpenWindow(setName, attributeNames, UpdateAttribute, CheckAttributeSetValid);
        }

        private void UpdateAttribute(string updatedAttributeSet, List<string> updatedAttributeNames)
        {
            updatedAttributeNames = EditorUtil.RemoveDuplicates(updatedAttributeNames);

            if (_selectedIndex >= 0 && _selectedIndex < Asset.AttributeSetConfigs.Count)
            {
                Asset.AttributeSetConfigs[_selectedIndex].Name = updatedAttributeSet;
                Asset.AttributeSetConfigs[_selectedIndex].AttributeNames = updatedAttributeNames;
            }
            else
            {
                Asset.AttributeSetConfigs.Add(new AttributeSetConfig
                {
                    Name = updatedAttributeSet,
                    AttributeNames = updatedAttributeNames
                });
            }

            Save();
        }
    }
}
#endif