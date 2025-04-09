using System;
using System.Collections.Generic;
using GAS.RuntimeDataHelper.GameplayEffect;
using GAS.RuntimeWithECS.Ability.Component;
using GAS.RuntimeWithECS.GameplayEffect;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigArrayGameplayEffect : AbilityParamConfigBase<AbilityParamArrayGameplayEffect>
    {
        public override AbilityParamBase GetConfig()
        {
            List<GameplayEffectConfig> configs = new List<GameplayEffectConfig>();
            foreach (var config in gameplayEffectConfigs)
            {
                configs.Add(config.GetConfig());
            }
            return new AbilityParamArrayGameplayEffect(configs.ToArray());
        }
        
        [LabelText("生效GE队列")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public List<GameplayEffectConfigAsset> gameplayEffectConfigs = new();
    }
}