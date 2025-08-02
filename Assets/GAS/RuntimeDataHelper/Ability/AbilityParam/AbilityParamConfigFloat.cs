using System;
using GAS.RuntimeWithECS;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigFloat: AbilityParamConfigBase<AbilityParamFloat>
    {
        public override IAbilityParam GetConfig()
        {
            return new AbilityParamFloat(value);
        }
        
        [LabelText("值")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public float value;
    }
}