using GAS.Core;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor.GameplayAbilitySystem
{
    public class GASSetting
    {
        private static GASSettingAsset _asset;
        private static UnityEditor.Editor _editor;

        [SettingsProvider]
        private static SettingsProvider AttributeSetManagerSetting()
        {
            var provider = new SettingsProvider("Project/EX Gameplay Ability System", SettingsScope.Project)
            {
                guiHandler = key => { SettingGUI(); },
                keywords = new[] { "GAS", "Setting" }
            };
            return provider;
        }

        private static void SettingGUI()
        {
            if (_editor == null) Load();

            EditorGUILayout.BeginVertical();
            _editor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }

        private static void Load()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GASSettingAsset>(GasDefine.GAS_SYSTEM_ASSET_PATH);
            if (asset == null)
            {
                GasDefine.CheckGasAssetFolder();

                var a = ScriptableObject.CreateInstance<GASSettingAsset>();
                AssetDatabase.CreateAsset(a, GasDefine.GAS_SYSTEM_ASSET_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                asset = ScriptableObject.CreateInstance<GASSettingAsset>();
            }

            _asset = asset;
            _editor = UnityEditor.Editor.CreateEditor(asset);
        }
    }
}