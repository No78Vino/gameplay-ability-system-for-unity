using GAS.General;
using GAS.Runtime;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace GAS.Editor
{
    public class GASCenterWindow:OdinMenuEditorWindow
    {
        private static GASSettingAsset _settingAsset;
        private static GASCenterViewTag _viewTag;
        private static GASCenterViewAttr _viewAttr;
        private static GASCenterViewAttrSet _viewAttrSet;
        private static GASCenterViewEffect _viewEffect;
        private static GASCenterViewCue _viewCue;
        private static GASCenterViewMmc _viewMmc;
        private static GASCenterViewAbility _viewAbility;
        private static GASCenterViewAsc _viewAsc;
        
        [MenuItem("EXTool/EX-GAS/GAS中心管理器")]
        public static void OpenWindow()
        {
            var window = GetWindow<GASCenterWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 600);
            GasJsonReader.ReadAllAndCache();
            GeneralGasChoiceHelper.LoadCache();
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            tree.Add("Setting基本设置",Setting());
            tree.Add("GameplayTag标签", GameplayTagEditor());
            tree.Add("Attribute属性", AttributeEditor());
            tree.Add("Attribute Set属性集", AttributeSetEditor());
            tree.Add("GameplayCue演出提示", GameplayCueEditor());
            tree.Add("MMC修改器", MMCEditor());
            tree.Add("GameplayEffect效果buff", GameplayEffectEditor());
            tree.Add("GameplayAbility技能", GameplayAbilityEditor());
            tree.Add("ASC预设", GameplayAbilityAscEditor());

            tree.Config.AutoScrollOnSelectionChanged = true;
            tree.Config.DrawScrollView = true;
            tree.Config.AutoHandleKeyboardNavigation = false;
            return tree;
        }
        
        private static GASSettingAsset Setting()
        {
            if (_settingAsset == null) _settingAsset = GASSettingAsset.LoadOrCreate();
            return _settingAsset;
        }
        
        private static GASCenterViewTag GameplayTagEditor()
        {
            return _viewTag ??= new GASCenterViewTag();
        }
        
        private static GASCenterViewAttr AttributeEditor()
        {
            return _viewAttr ??= new GASCenterViewAttr();
        }
        
        private static GASCenterViewAttrSet AttributeSetEditor()
        {
            return _viewAttrSet ??= new GASCenterViewAttrSet();
        }
        
        private static GASCenterViewEffect GameplayEffectEditor()
        {
            return _viewEffect ??= new GASCenterViewEffect();
        }
        
        private static GASCenterViewCue GameplayCueEditor()
        {
            return _viewCue ??= new GASCenterViewCue();
        }
        
        private static GASCenterViewMmc MMCEditor()
        {
            return _viewMmc ??= new GASCenterViewMmc();
        }
        
        private static GASCenterViewAbility GameplayAbilityEditor()
        {
            return _viewAbility ??= new GASCenterViewAbility();
        }

        private static GASCenterViewAsc GameplayAbilityAscEditor()
        {
            return _viewAsc ??= new GASCenterViewAsc();
        }
    }
}