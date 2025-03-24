using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetAbilityActivationRequiredTags : BaseGameplayAbilityComponentConfigAsset
    {
        [SerializeField] 
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