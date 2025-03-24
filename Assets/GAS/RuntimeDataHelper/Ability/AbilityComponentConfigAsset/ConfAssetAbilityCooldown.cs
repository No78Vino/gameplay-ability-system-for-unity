using System;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetAbilityCooldown:BaseGameplayAbilityComponentConfigAsset
    {
        public float cooldown = 0.0f;
        
        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfAbilityCooldown
            {
                Cooldown = cooldown
            };
        }
    }
}