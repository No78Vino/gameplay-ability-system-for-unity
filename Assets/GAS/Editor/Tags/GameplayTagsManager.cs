#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using UnityEngine;

    public class GameplayTagsManager
    {
        private static GameplayTagsAsset _asset;
        private static Editor _editor;
        
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
            var asset = GameplayTagsAsset.LoadOrCreate();
            _asset = asset;
            _editor = Editor.CreateEditor(asset);
        }
    }
}
#endif