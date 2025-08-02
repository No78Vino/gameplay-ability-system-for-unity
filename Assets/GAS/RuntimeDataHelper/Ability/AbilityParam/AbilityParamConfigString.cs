using System;
using GAS.RuntimeWithECS;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigString: AbilityParamConfigBase<AbilityParamString>
    {
        public override IAbilityParam GetConfig()
        {
            return new AbilityParamString(value);
        }
        
        [LabelText("值")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public string value;
    }
}