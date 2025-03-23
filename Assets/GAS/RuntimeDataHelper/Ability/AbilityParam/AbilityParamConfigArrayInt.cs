using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigArrayInt: AbilityParamConfigBase
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamArrayInt(value.ToArray());
        }
        
        public List<int> value=new();
    }
}