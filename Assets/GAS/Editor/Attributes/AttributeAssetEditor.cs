#if UNITY_EDITOR
namespace GAS.Editor.Attributes
{
    using System.Collections.Generic;
    using General;
    using Runtime.Attribute;
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(AttributeAsset))]
    public class AttributeAssetEditor:Editor
    {
        private AttributeAsset Asset => (AttributeAsset)target;
        
        public override void OnInspectorGUI()
        {
            ToolBar();
            GUILayout.Space(3f);
            
            for (var i = 0; i < AttributeList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // Show the string
                EditorGUILayout.LabelField($"No.{i} : {AttributeList[i]}");

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
            StringEditWindow.OpenWindow("",AddAttribute,"Attribute");
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

             if (!EditorUtil.IsValidClassName(attributeName))
             {
                 EditorUtility.DisplayDialog("Warning","Invalid attribute name!Please check the naming rules.","OK");
                 return;
             }
             Asset.AttributeNames.Add(attributeName);
             Save();
         }
         
         private int _selectedIndex = -1;
         private List<string> AttributeList => Asset.AttributeNames;
         
         private void OpenEditPopup()
         {
             if (_selectedIndex < 0 || _selectedIndex >= AttributeList.Count) return;
             StringEditWindow.OpenWindow(AttributeList[_selectedIndex], UpdateAttribute,"Attribute");
         }

         private void UpdateAttribute(string updatedAttribute)
         {
             if (_selectedIndex >= 0 && _selectedIndex < AttributeList.Count)
                 AttributeList[_selectedIndex] = updatedAttribute;
         }
    }
}
#endif