using System;
using GAS.RuntimeWithECS.Ability.Component;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigNone: AbilityParamConfigBase
    {
        public override AbilityParamBase GetConfig()
        {
            return AbilityParamNone.None;
        }
    }
}