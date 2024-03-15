
#if UNITY_EDITOR
namespace GAS.Editor
{
    using Core;
    using UnityEditor;
    using UnityEngine;
    using Editor;

    public class GameplayTagsManager
    {
        private static GameplayTagsAsset _asset;
        private static UnityEditor.Editor _editor;
        
        [SettingsProvider]
        private static SettingsProvider GameplayTagsManagerSetting()
        {
            var provider = new SettingsProvider("Project/EX Gameplay Ability System/Tag Manager", SettingsScope.Project)
            {
                guiHandler = key => { SettingGUI(); },
                keywords = new string[] { "GAS","Tag" }
            };
            return provider;
        }

        private static void SettingGUI()
        {
            if(_asset == null) Load();
            if (_editor == null) return;
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            EditorGUILayout.Space();
            _editor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }

        private static void Load()
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameplayTagsAsset>(GASSettingAsset.GAS_TAG_ASSET_PATH);
            // if (asset == null)
            // {
            //     GasDefine.CheckGasAssetFolder();
            //
            //     var a = ScriptableObject.CreateInstance<GameplayTagsAsset>();
            //     AssetDatabase.CreateAsset(a, GASSettingAsset.GAS_TAG_ASSET_PATH);
            //     AssetDatabase.SaveAssets();
            //     AssetDatabase.Refresh();
            //     asset = ScriptableObject.CreateInstance<GameplayTagsAsset>();
            // }

            _asset = asset;
            _editor = UnityEditor.Editor.CreateEditor(asset);
        }
    }
}
#endif