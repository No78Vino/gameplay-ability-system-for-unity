using System;
using GAS.RuntimeWithECS.Static;
using GAS.RuntimeWithECS.ComponentConfig;

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