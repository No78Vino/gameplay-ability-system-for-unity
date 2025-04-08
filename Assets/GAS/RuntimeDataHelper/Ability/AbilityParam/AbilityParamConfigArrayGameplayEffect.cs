using System;
using GAS.RuntimeWithECS.Ability.Component;
using GAS.RuntimeWithECS.GameplayEffect;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigArrayGameplayEffect : AbilityParamConfigBase<AbilityParamArrayGameplayEffect>
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamArrayGameplayEffect(Array.Empty<GameplayEffectConfig>());
        }
        
        // TODO 添加GE的配置
    }
}