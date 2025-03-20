using System;
using GAS.RuntimeWithECS.Ability.ComponentConfig;

namespace GAS.RuntimeDataHelper.Ability
{
    [Serializable]
    public abstract class BaseGameplayAbilityComponentConfigAsset
    {
        public abstract GameplayAbilityComponentConfig GetConfig();
    }
}