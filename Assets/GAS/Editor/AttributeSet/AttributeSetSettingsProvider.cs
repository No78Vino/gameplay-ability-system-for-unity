#if UNITY_EDITOR
namespace GAS.Editor
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    public class AttributeSetSettingsProvider: SettingsProvider
    {
        private  AttributeSetAsset _asset;
        private  Editor _editor;
        
        public AttributeSetSettingsProvider() : base("Project/EX Gameplay Ability System/AttributeSet Manager", SettingsScope.Project)
        {
        }
        
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var asset = AttributeSetAsset.LoadOrCreate();
            _asset = asset;
            _editor = Editor.CreateEditor(asset);
            GASSettingStatusWatcher.OnEditorFocused += OnEditorFocused;
        }
        
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            GASSettingStatusWatcher.OnEditorFocused -= OnEditorFocused;
            AttributeSetAsset.Save();
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

        static AttributeSetSettingsProvider provider;
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (AttributeSetAsset.Instance && provider == null)
            {
                provider = new AttributeSetSettingsProvider();
                using (var so = new SerializedObject(AttributeSetAsset.Instance))
                {
                    provider.keywords = GetSearchKeywordsFromSerializedObject(so);
                }
            }
            return provider;
        }
    }
}
#endif