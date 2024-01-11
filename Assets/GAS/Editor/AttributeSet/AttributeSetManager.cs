
#if UNITY_EDITOR
namespace GAS.Editor.AttributeSet
{
    using GAS.Core;
    using GAS.Runtime.AttributeSet;
    using UnityEditor;
    using UnityEngine;
    using GAS.Editor.GameplayAbilitySystem;

    public class AttributeSetManager
    {
        private static AttributeSetAsset _asset;
        private static Editor _editor;
        
        [SettingsProvider]
        private static SettingsProvider AttributeSetManagerSetting()
        {
            var provider = new SettingsProvider("Project/EX Gameplay Ability System/AttributeSet Manager", SettingsScope.Project)
            {
                guiHandler = key => { SettingGUI(); },
                keywords = new string[] { "GAS","Attribute","AttributeSet" }
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
            var asset = AssetDatabase.LoadAssetAtPath<AttributeSetAsset>(GASSettingAsset.GAS_ATTRIBUTESET_ASSET_PATH);
            if (asset == null)
            {
                GasDefine.CheckGasAssetFolder();

                var a = ScriptableObject.CreateInstance<AttributeSetAsset>();
                AssetDatabase.CreateAsset(a, GASSettingAsset.GAS_ATTRIBUTESET_ASSET_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                asset = ScriptableObject.CreateInstance<AttributeSetAsset>();
            }

            _asset = asset;
            _editor = UnityEditor.Editor.CreateEditor(asset);
        }
    }
}
#endif