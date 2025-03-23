using System;
using GAS.RuntimeWithECS.Ability.Component;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigInt: AbilityParamConfigBase
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamInt(value);
        }
        
        public int value;
    }
}