using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigArrayFloat: AbilityParamConfigBase
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamArrayFloat(value.ToArray());
        }
        
        [LabelText("值")]
        public List<float> value=new();
    }
}