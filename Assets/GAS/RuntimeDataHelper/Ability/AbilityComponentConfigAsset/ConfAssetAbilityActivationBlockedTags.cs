using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetAbilityActivationBlockedTags:BaseGameplayAbilityComponentConfigAsset
    {
        [TabGroup(
            "AbilityActivationBlockedTags",
            "有以下标签时，能力激活被阻断",
            SdfIconType.TagsFill, TextColor = "#99B188", Order = 1)]
        [SerializeField] 
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> tags = new();
        
        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfAbilityActivationBlockedTags
            {
                tags = tags.ToArray()
            };
        }
    }
}