using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GAS.Editor.General;
using GAS.Runtime.Attribute;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Attributes
{
    public class StringEditWindow : EditorWindow
    {
        private Action<string> callback;
        private string editedString = "";

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Edit", EditorStyles.boldLabel);

            // Display the input field to edit the string
            editedString = EditorGUILayout.TextField("Attribute:", editedString);

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
    
    [CustomEditor(typeof(AttributeAsset))]
    public class AttributeAssetEditor:UnityEditor.Editor
    {
        private AttributeAsset Asset => (AttributeAsset)target;
        
        public override void OnInspectorGUI()
        {
            Asset.AttributeCollectionGenPath = EditorGUILayout.TextField("Code Gen Path", Asset.AttributeCollectionGenPath);
            GUILayout.Space(5f);
            ToolBar();
            GUILayout.Space(3f);
            
            for (var i = 0; i < AttributeList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // Show the string
                EditorGUILayout.LabelField($"Attribute {i}: {AttributeList[i]}");

                // Edit button to modify the selected string
                if (GUILayout.Button("Edit", GUILayout.Width(50)))
                {
                    _selectedIndex = i;
                    OpenEditPopup();
                }

                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    _selectedIndex = i;
                    Remove(AttributeList[i]);
                }
                
                EditorGUILayout.EndHorizontal();
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

        void Add()
        {
            InputStringWindow.OpenWindow("Add Attribute",AddAttribute);
        }
        
        void Remove(string attributeName)
        {
            var result = EditorUtility.DisplayDialog("Confirmation", $"Are you sure you want to REMOVE Attribute:{attributeName}?",
                "Yes", "No");

            if (!result) return;
            Asset.AttributeNames.Remove(attributeName);
            Save();
        }
        
        void Save()
        {
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
        }

         void GenCode()
         {
             Save();
             AttributeCollectionGen.Gen();
             AssetDatabase.Refresh();
         }
         
         void AddAttribute(string attributeName)
         {
             if (Asset.AttributeNames.Contains(attributeName))
             {
                 EditorUtility.DisplayDialog("Warning","Attribute name already exists!","OK");
                 return;
             }

             if (!IsValidClassName(attributeName))
             {
                 EditorUtility.DisplayDialog("Warning","Invalid attribute name!Please check the naming rules.","OK");
                 return;
             }
             Asset.AttributeNames.Add(attributeName);
             Save();
         }
         
         public static bool IsValidClassName(string input)
         {
             // 使用正则表达式匹配规则
             // 类名必须以字母、下划线或@开头，并且后续可以是字母、下划线、@或数字
             string pattern = @"^[a-zA-Z_@][a-zA-Z_@0-9]*$";

             // 使用 Regex.IsMatch 方法进行匹配
             return Regex.IsMatch(input, pattern);
         }
         
         private int _selectedIndex = -1;
         private List<string> AttributeList => Asset.AttributeNames;
         
         private void OpenEditPopup()
         {
             if (_selectedIndex < 0 || _selectedIndex >= AttributeList.Count) return;
             var window = CreateInstance<StringEditWindow>();
             window.Init(AttributeList[_selectedIndex], UpdateAttribute);
             window.ShowUtility();
         }

         private void UpdateAttribute(string updatedAttribute)
         {
             if (_selectedIndex >= 0 && _selectedIndex < AttributeList.Count)
                 AttributeList[_selectedIndex] = updatedAttribute;
         }
    }
}