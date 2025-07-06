using GAS.Runtime;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace GAS.Editor
{
    public class GASCenterWindow:OdinMenuEditorWindow
    {
        private static GASSettingAsset _settingAsset;
        
        [MenuItem("EXTool/EX-GAS/GAS中心管理器")]
        public static void OpenWindow()
        {
            var window = GetWindow<GASCenterWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 600);
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            tree.Add("Setting基本设置",Setting());
            tree.Add("GameplayTag标签", GameplayTagEditor());
            tree.Add("Attribute属性", AttributeEditor());
            tree.Add("Attribute Set属性集", AttributeSetEditor());
            tree.Add("GameplayEffect效果buff", GameplayEffectEditor());
            tree.Add("GameplayAbility技能", GameplayAbilitySystemEditor());

            tree.Config.AutoScrollOnSelectionChanged = true;
            tree.Config.DrawScrollView = true;
            tree.Config.AutoHandleKeyboardNavigation = true;
            tree.Selection.SelectionChanged += type =>
            {
                // GASSettingAsset.Save();
                // GameplayTagsAsset.Save();
                // AttributeAsset.Save();
                // AttributeSetAsset.Save();
            };
            return tree;
        }
        
        private static GASSettingAsset Setting()
        {
            if (_settingAsset == null) _settingAsset = GASSettingAsset.LoadOrCreate();
            return _settingAsset;
        }
        
        private static GASSettingAsset GameplayTagEditor()
        {
            if (_settingAsset == null) _settingAsset = GASSettingAsset.LoadOrCreate();
            return _settingAsset;
        }
        
        private static GASSettingAsset AttributeEditor()
        {
            if (_settingAsset == null) _settingAsset = GASSettingAsset.LoadOrCreate();
            return _settingAsset;
        }
        
        private static GASSettingAsset AttributeSetEditor()
        {
            if (_settingAsset == null) _settingAsset = GASSettingAsset.LoadOrCreate();
            return _settingAsset;
        }
        
        private static GASSettingAsset GameplayEffectEditor()
        {
            if (_settingAsset == null) _settingAsset = GASSettingAsset.LoadOrCreate();
            return _settingAsset;
        }
        
        private static GASSettingAsset GameplayAbilitySystemEditor()
        {
            if (_settingAsset == null) _settingAsset = GASSettingAsset.LoadOrCreate();
            return _settingAsset;
        }
    }
}