using System;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    // TODO
    [Serializable]
    public class ConfAssetAbilityCost:BaseGameplayAbilityComponentConfigAsset
    {
        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfAbilityCost
            {
            };
        }
    }
}