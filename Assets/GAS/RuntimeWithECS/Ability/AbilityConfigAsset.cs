using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeWithECS.Ability
{
    [CreateAssetMenu(fileName = "AbilityConfigAsset", menuName = "EX-GAS/Ability", order = 0)]
    public class AbilityConfigAsset : ScriptableObject
    {
        [ShowInInspector]
        public List<IGameplayAbilityComponentConfigAsset> componentConfigs;
        
        public AbilityConfig GetConfig()
        {
            List<GameplayAbilityComponentConfig> configs = new List<GameplayAbilityComponentConfig>();
            foreach (var config in componentConfigs)
            {
                configs.Add(config.GetConfig());
            }
            return new AbilityConfig(configs.ToArray());
        }
    }
}