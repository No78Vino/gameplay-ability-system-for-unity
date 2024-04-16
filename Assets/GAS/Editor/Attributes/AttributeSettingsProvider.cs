#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    public class AttributeSettingsProvider: SettingsProvider
    {
        private  AttributeAsset _asset;
        private  Editor _editor;
        
        public AttributeSettingsProvider() : base("Project/EX Gameplay Ability System/Attribute Manager", SettingsScope.Project)
        {
        }
        
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var asset = AttributeAsset.LoadOrCreate();
            _asset = asset;
            _editor = Editor.CreateEditor(asset);
            GASSettingStatusWatcher.OnEditorFocused += OnEditorFocused;
        }
        
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            GASSettingStatusWatcher.OnEditorFocused -= OnEditorFocused;
            AttributeAsset.Save();
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

        static AttributeSettingsProvider provider;
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (AttributeAsset.Instance && provider == null)
            {
                provider = new AttributeSettingsProvider();
                using (var so = new SerializedObject(AttributeAsset.Instance))
                {
                    provider.keywords = GetSearchKeywordsFromSerializedObject(so);
                }
            }
            return provider;
        }
    }
}
#endif