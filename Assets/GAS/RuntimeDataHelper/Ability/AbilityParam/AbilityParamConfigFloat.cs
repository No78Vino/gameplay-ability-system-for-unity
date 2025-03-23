using System;
using GAS.RuntimeWithECS.Ability.Component;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigFloat: AbilityParamConfigBase
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamFloat(value);
        }
        
        public float value;
    }
}