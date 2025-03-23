using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigArrayInt: AbilityParamConfigBase<AbilityParamArrayInt>
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamArrayInt(value.ToArray());
        }
        
        [LabelText("值")]
        public List<int> value=new();
    }
}