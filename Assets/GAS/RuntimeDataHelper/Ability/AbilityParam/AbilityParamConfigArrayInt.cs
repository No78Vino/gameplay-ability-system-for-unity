using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigArrayInt: AbilityParamConfigBase<AbilityParamArrayInt>
    {
        public override IAbilityParam GetConfig()
        {
            return new AbilityParamArrayInt(value.ToArray());
        }
        
        [LabelText("值")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public List<int> value=new();
    }
}