using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigArrayFloat: AbilityParamConfigBase<AbilityParamArrayFloat>
    {
        public override IAbilityParam GetConfig()
        {
            return new AbilityParamArrayFloat(value.ToArray());
        }
        
        [LabelText("值")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public List<float> value=new();
    }
}