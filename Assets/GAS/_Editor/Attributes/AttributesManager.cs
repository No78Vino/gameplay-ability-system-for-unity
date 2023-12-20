using GAS.Core;
using GAS.Runtime.Attribute;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Attributes
{
    public class AttributesManager
    {
        private static AttributeAsset _asset;
        private static UnityEditor.Editor _editor;
        
        [SettingsProvider]
        private static SettingsProvider GameplayTagsManagerSetting()
        {
            var provider = new SettingsProvider("Project/EX Gameplay Ability System/Attribute Manager", SettingsScope.Project)
            {
                guiHandler = key => { SettingGUI(); },
                keywords = new string[] { "GAS","Attribute" }
            };
            return provider;
        }

        private static void SettingGUI()
        {
            if (_editor == null) Load();
            
            EditorGUILayout.BeginVertical(GUI.skin.box,GUILayout.Width(700));
            _editor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }

        private static void Load()
        {
            var asset = AssetDatabase.LoadAssetAtPath<AttributeAsset>(GasDefine.GAS_ATTRIBUTE_ASSET_PATH);
            if (asset == null)
            {
                GasDefine.CheckGasAssetFolder();

                var a = ScriptableObject.CreateInstance<AttributeAsset>();
                AssetDatabase.CreateAsset(a, GasDefine.GAS_ATTRIBUTE_ASSET_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                asset = ScriptableObject.CreateInstance<AttributeAsset>();
            }

            _asset = asset;
            _editor = UnityEditor.Editor.CreateEditor(asset);
        }
    }
}