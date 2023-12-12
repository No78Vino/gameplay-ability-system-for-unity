using GAS.Core;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.Tags
{
    public class GameplayTagsManager
    {
        private static UnityEditor.Editor _editor;

        [SettingsProvider]
        private static SettingsProvider GameplayTagsManagerSetting()
        {
            var provider = new SettingsProvider("Project/EX Gameplay Ability System/Tag Manager", SettingsScope.Project)
            {
                guiHandler = key => { SettingGUI(); },
                keywords = new string[] { }
            };
            return provider;
        }

        private static void SettingGUI()
        {
            if (_editor == null) Load();

            _editor.OnInspectorGUI();
        }

        private static void Load()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameplayTagsAsset>(GasDefine.GAS_TAG_ASSET_PATH);
            if (asset == null)
            {
                GasDefine.CheckGasAssetFolder();

                var a = ScriptableObject.CreateInstance<GameplayTagsAsset>();
                AssetDatabase.CreateAsset(a, GasDefine.GAS_TAG_ASSET_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                asset = ScriptableObject.CreateInstance<GameplayTagsAsset>();
            }

            _editor = UnityEditor.Editor.CreateEditor(asset);
        }
    }
}