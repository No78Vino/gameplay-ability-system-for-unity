using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GAS.Editor.Attributes;
using GAS.Editor.General;
using GAS.Runtime.AttributeSet;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.AttributeSet
{
    public class AttributeSetConfigEditorWindow : EditorWindow
    {
        private Action<string,string[]> callback;
        
        
        [LabelText("AttributeSet Name")]
        public string editedName = "";
        
       
        [TableList(AlwaysExpanded = true,ShowIndexLabels = true,NumberOfItemsPerPage = 10)]
        public string[] _attributeNames;
        
        [Button]
        void Save()
        {
            callback?.Invoke(editedName,_attributeNames);
            Close();
        }
        
        public void Init(string initialString,string[] attributeNames, Action<string,string[]> callback)
        {
            editedName = initialString;
            this.callback = callback;
            _attributeNames = attributeNames;
        }
    }
    
    [CustomEditor(typeof(AttributeSetAsset))]
    public class AttributeSetAssetEditor:UnityEditor.Editor
    {
        private AttributeSetAsset Asset => (AttributeSetAsset)target;
        private int _selectedIndex = -1;
        private List<AttributeSetConfig> AttributeSetsList => Asset.AttributeSets;
        
        public override void OnInspectorGUI()
        {
            Asset.AttributeSetCollectionGenPath = EditorGUILayout.TextField("Code Gen Path", Asset.AttributeSetCollectionGenPath);
            GUILayout.Space(5f);
            ToolBar();
            GUILayout.Space(3f);
            
            for (var i = 0; i < AttributeSetsList.Count; i++)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Attribute Set {i}: {AttributeSetsList[i].Name}");

                // Edit button to modify the selected string
                if (GUILayout.Button("Edit", GUILayout.Width(50)))
                {
                    _selectedIndex = i;
                    OpenEditPopup();
                }

                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    _selectedIndex = i;
                    //Remove(AttributeSetsList[i]);
                }
                
                EditorGUILayout.EndHorizontal();
                
                
                foreach (var t in AttributeSetsList[i].AttributeNames)
                {
                    EditorGUILayout.LabelField($"   Attribute: {t}");
                }
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

        void Add()
        {
            InputStringWindow.OpenWindow("Add AttributeSet",AddAttributeSet);
        }
        
        void Remove(string attributeName)
        {
            // var result = EditorUtility.DisplayDialog("Confirmation", $"Are you sure you want to REMOVE Attribute:{attributeName}?",
            //     "Yes", "No");
            //
            // if (!result) return;
            // Asset.AttributeNames.Remove(attributeName);
            // Save();
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
         
         void AddAttributeSet(string attributeSetName)
         {
             // if (Asset.AttributeSets.Contains(attributeSetName))
             // {
             //     EditorUtility.DisplayDialog("Warning","AttributeSet name already exists!","OK");
             //     return;
             // }

             if (!IsValidClassName(attributeSetName))
             {
                 EditorUtility.DisplayDialog("Warning","Invalid attributeset name!Please check the naming rules.","OK");
                 return;
             }
             Asset.AttributeSets.Add(new AttributeSetConfig()
             {
                 Name = attributeSetName,
                 AttributeNames = new string[0]
             });
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
         
         private void OpenEditPopup()
         {
             if (_selectedIndex < 0 || _selectedIndex >= AttributeSetsList.Count) return;
             var window = CreateInstance<AttributeSetConfigEditorWindow>();
             window.Init(AttributeSetsList[_selectedIndex].Name,
                 AttributeSetsList[_selectedIndex].AttributeNames, 
                 UpdateAttribute);
             window.ShowUtility();
         }

         private void UpdateAttribute(string updatedAttributeSet, string[] updatedAttributeNames)
         {
             if (_selectedIndex >= 0 && _selectedIndex < AttributeSetsList.Count)
             {
                 AttributeSetsList[_selectedIndex].SetName(updatedAttributeSet);
                 AttributeSetsList[_selectedIndex].SetAttributeNames(updatedAttributeNames);
             }
         }
    }
}