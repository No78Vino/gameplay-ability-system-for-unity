using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace GAS.Editor
{
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
                if (_settingAsset == null) _settingAsset = GASSettingAsset.LoadOrCreate();
                return _settingAsset;
            }
        }

        private static GameplayTagsAsset TagsAsset
        {
            get
            {
                if (_tagsAsset == null) _tagsAsset = GameplayTagsAsset.LoadOrCreate();
                return _tagsAsset;
            }
        }

        private static AttributeAsset AttributeAsset
        {
            get
            {
                if (_attributeAsset == null) _attributeAsset = AttributeAsset.LoadOrCreate();
                return _attributeAsset;
            }
        }

        private static AttributeSetAsset AttributeSetAsset
        {
            get
            {
                if (_attributeSetAsset == null) _attributeSetAsset = AttributeSetAsset.LoadOrCreate();
                return _attributeSetAsset;
            }
        }

        private const string OpenWindow_MenuItemName = "EX-GAS/Settings";
#if EX_GAS_ENABLE_HOT_KEYS
        private const string OpenWindow_MenuItemNameEnh = OpenWindow_MenuItemName + " %F12";
#else
        private const string OpenWindow_MenuItemNameEnh = OpenWindow_MenuItemName;
#endif
        [MenuItem(OpenWindow_MenuItemNameEnh, priority = 0)]
        public static void OpenWindow()
        {
            var window = GetWindow<GASSettingAggregator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 600);
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
            tree.Selection.SelectionChanged += type =>
            {
                GASSettingAsset.Save();
                GameplayTagsAsset.Save();
                AttributeAsset.Save();
                AttributeSetAsset.Save();
            };
            return tree;
        }
    }
}