using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Static;
using GAS.RuntimeWithECS.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetAbilityActivationRequiredTags : BaseGameplayAbilityComponentConfigAsset
    {
        [TabGroup(
            "ActivationRequiredTags",
            "能力激活所需标签",
            SdfIconType.TagsFill, TextColor = "#33B188", Order = 1)]
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> tags = new();
        
        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfAbilityActivationRequiredTags
            {
                tags = tags.ToArray()
            };
        }
    }
}