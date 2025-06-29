using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigArrayGameplayEffect : AbilityParamConfigBase<AbilityParamArrayGameplayEffect>
    {
        [FormerlySerializedAs("gameplayEffectConfigs")]
        [LabelText("生效GE队列")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        [FilePath(IncludeFileExtension = false, Extensions = "asset")]
        public List<string> gameplayEffectConfigPaths = new();

        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamArrayGameplayEffect(gameplayEffectConfigPaths.ToArray());
        }
    }
}