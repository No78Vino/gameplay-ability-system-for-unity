using System;
using System.Collections.Generic;
using System.Linq;
using GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability
{
    public class GEN_AbilityConfigtSO:ScriptableObject
    {
        [TabGroup("AbilityConfig","能力组件类型控制",SdfIconType.TagsFill)]
        [ValueDropdown("@EXEditorHelper.AbilityComponentTypeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<string> configTypes = new();
        
        [TabGroup("AbilityConfig","组件配置详情",SdfIconType.Activity)]
        [LabelText("基础信息")]
        [ShowIf(nameof(HasConfAssetAbilityBaseInfo))]
        [OnValueChanged(nameof(OnConfigValueChanged))]
        public ConfAssetAbilityBaseInfo ConfAssetAbilityBaseInfo;

        [TabGroup("AbilityConfig","组件配置详情")]
        [LabelText("能力逻辑")]
        [ShowIf(nameof(HasMCConfAssetAbilityLogic))]
        public MCConfAssetAbilityLogic MCConfAssetAbilityLogic;
        
        private bool HasConfAssetAbilityBaseInfo => 
            configTypes.Any( x => x == typeof(ConfAssetAbilityBaseInfo).FullName);
        
        private bool HasMCConfAssetAbilityLogic() =>
            configTypes.Any(x => x == typeof(MCConfAssetAbilityLogic).FullName);

        private void OnConfigValueChanged()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}