
#if  UNITY_EDITOR
namespace GAS.Editor.Attributes
{
    using GAS.Core;
    using GAS.Runtime.Attribute;
    using UnityEditor;
    using UnityEngine;
    using GAS.Editor.GameplayAbilitySystem;

    public class AttributesManager
    {
        private static AttributeAsset _asset;
        private static Editor _editor;
        
        [SettingsProvider]
        private static SettingsProvider AttributeManagerSetting()
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
            var asset = AssetDatabase.LoadAssetAtPath<AttributeAsset>(GASSettingAsset.GAS_ATTRIBUTE_ASSET_PATH);
            if (asset == null)
            {
                GasDefine.CheckGasAssetFolder();

                var a = ScriptableObject.CreateInstance<AttributeAsset>();
                AssetDatabase.CreateAsset(a, GASSettingAsset.GAS_ATTRIBUTE_ASSET_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                asset = ScriptableObject.CreateInstance<AttributeAsset>();
            }

            _asset = asset;
            _editor = UnityEditor.Editor.CreateEditor(asset);
        }
    }
}
#endif