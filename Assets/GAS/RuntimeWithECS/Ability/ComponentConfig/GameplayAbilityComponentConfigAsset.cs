using UnityEngine;

namespace GAS.RuntimeWithECS.Ability.ComponentConfig
{
    public interface IGameplayAbilityComponentConfigAsset
    {
        public GameplayAbilityComponentConfig GetConfig();
    }
}