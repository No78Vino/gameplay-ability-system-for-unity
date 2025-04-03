using System;
using GAS.RuntimeWithECS.Ability.Component;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigFloat: AbilityParamConfigBase<AbilityParamFloat>
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamFloat(value);
        }
        
        [LabelText("值")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public float value;
    }
}