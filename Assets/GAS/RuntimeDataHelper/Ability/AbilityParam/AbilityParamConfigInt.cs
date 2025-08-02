using System;
using GAS.RuntimeWithECS;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigInt: AbilityParamConfigBase<AbilityParamInt>
    {
        public override IAbilityParam GetConfig()
        {
            return new AbilityParamInt(value);
        }
        
        [LabelText("值")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public int value;
    }
}