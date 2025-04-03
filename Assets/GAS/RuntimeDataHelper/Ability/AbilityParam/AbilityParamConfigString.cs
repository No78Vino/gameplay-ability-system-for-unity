using System;
using GAS.RuntimeWithECS.Ability.Component;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigString: AbilityParamConfigBase<AbilityParamString>
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamString(value);
        }
        
        [LabelText("值")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public string value;
    }
}