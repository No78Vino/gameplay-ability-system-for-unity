#if UNITY_EDITOR
namespace GAS.Editor.GameplayAbilitySystem
{
    using GAS.Core;
    using GAS.Editor.Attribute;
    using GAS.Runtime.AttributeSet;
    using GAS.Runtime.Tags;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    
    public class GASSettingAggregator : OdinMenuEditorWindow
    {
        private static GASSettingAsset _settingAsset;

        private static GameplayTagsAsset _tagsAsset;

        private static AttributeAsset _attributeAsset;

        private static AttributeSetAsset _attributeSetAsset;

        private static GASSettingAsset SettingAsset
        {
            get
            {
                if (_settingAsset == null)
                    _settingAsset = AssetDatabase.LoadAssetAtPath<GASSettingAsset>(GasDefine.GAS_SYSTEM_ASSET_PATH);
                return _settingAsset;
            }
        }

        private static GameplayTagsAsset TagsAsset
        {
            get
            {
                if (_tagsAsset == null)
                    _tagsAsset = AssetDatabase.LoadAssetAtPath<GameplayTagsAsset>(GASSettingAsset.GAS_TAG_ASSET_PATH);
                return _tagsAsset;
            }
        }

        private static AttributeAsset AttributeAsset
        {
            get
            {
                if (_attributeAsset == null)
                    _attributeAsset =
                        AssetDatabase.LoadAssetAtPath<AttributeAsset>(GASSettingAsset.GAS_ATTRIBUTE_ASSET_PATH);
                return _attributeAsset;
            }
        }

        private static AttributeSetAsset AttributeSetAsset
        {
            get
            {
                if (_attributeSetAsset == null)
                    _attributeSetAsset =
                        AssetDatabase.LoadAssetAtPath<AttributeSetAsset>(GASSettingAsset.GAS_ATTRIBUTESET_ASSET_PATH);
                return _attributeSetAsset;
            }
        }

        [MenuItem("EX-GAS/Settings", priority = 0)]
        private static void OpenWindow()
        {
            var window = GetWindow<GASSettingAggregator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(900, 600);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            tree.Add("Setting", SettingAsset);
            tree.Add("Tags", TagsAsset);
            tree.Add("Attribute", AttributeAsset);
            tree.Add("Attribute Set", AttributeSetAsset);

            tree.Config.AutoScrollOnSelectionChanged = true;
            tree.Config.DrawScrollView = true;
            tree.Config.AutoHandleKeyboardNavigation = true;

            return tree;
        }
    }
}
#endif