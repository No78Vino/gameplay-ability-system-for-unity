using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetAbilityActivationOwnedTags : BaseGameplayAbilityComponentConfigAsset
    {
        [SerializeField] [ListDrawerSettings] public List<int> tags = new();

        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfAbilityActivationOwnedTags
            {
                tags = tags.ToArray()
            };
        }
    }
}