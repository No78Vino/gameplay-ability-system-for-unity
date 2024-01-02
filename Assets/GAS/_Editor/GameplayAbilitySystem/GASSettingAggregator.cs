using GAS.Core;
using GAS.Runtime.Attribute;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Tags;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace GAS.Editor.GameplayAbilitySystem
{
    public class GASSettingAggregator: OdinMenuEditorWindow
    {
        static GASSettingAsset _settingAsset;
        static GameplayTagsAsset _tagsAsset;
        static AttributeAsset _attributeAsset;
        static AttributeSetAsset _attributeSetAsset;

        [MenuItem("EX-GAS/Settings", priority = 0)]
        private static void OpenWindow()
        {
            CheckSettingAssets();
            var window = GetWindow<GASSettingAggregator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(900, 600);
        }

        static void CheckSettingAssets()
        {
            _settingAsset = AssetDatabase.LoadAssetAtPath<GASSettingAsset>(GasDefine.GAS_SYSTEM_ASSET_PATH);
            _tagsAsset = AssetDatabase.LoadAssetAtPath<GameplayTagsAsset>(GasDefine.GAS_TAG_ASSET_PATH);
            _attributeAsset = AssetDatabase.LoadAssetAtPath<AttributeAsset>(GasDefine.GAS_ATTRIBUTE_ASSET_PATH);
            _attributeSetAsset = AssetDatabase.LoadAssetAtPath<AttributeSetAsset>(GasDefine.GAS_ATTRIBUTESET_ASSET_PATH);
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            tree.Add("Setting", _settingAsset);
            tree.Add("Tags", _tagsAsset);
            tree.Add("Attribute", _attributeAsset);
            tree.Add("Attribute Set", _attributeSetAsset);
            
            tree.Config.AutoScrollOnSelectionChanged = true;
            tree.Config.DrawScrollView = true;
            tree.Config.AutoHandleKeyboardNavigation = true;
            
            return tree;
        }
    }
}