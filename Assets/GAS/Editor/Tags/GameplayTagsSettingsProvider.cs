#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    
    public class GameplayTagsSettingsProvider : SettingsProvider
    {
        private  Editor _editor;
        
        public GameplayTagsSettingsProvider() : base("Project/EX Gameplay Ability System/Tag Manager", SettingsScope.Project)
        {
        }
        
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _editor = Editor.CreateEditor(GameplayTagsAsset.LoadOrCreate());
            GASSettingStatusWatcher.OnEditorFocused += OnEditorFocused;
        }
        
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            GASSettingStatusWatcher.OnEditorFocused -= OnEditorFocused;
            GameplayTagsAsset.Save();
        }
        
        private void OnEditorFocused()
        {
            Repaint();
        }

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);

            if (_editor == null) return;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.Space();
            _editor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }

        static GameplayTagsSettingsProvider provider;
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (GameplayTagsAsset.Instance && provider == null)
            {
                provider = new GameplayTagsSettingsProvider();
                using (var so = new SerializedObject(GameplayTagsAsset.Instance))
                {
                    provider.keywords = GetSearchKeywordsFromSerializedObject(so);
                }
            }
            return provider;
        }
    }
}
#endif