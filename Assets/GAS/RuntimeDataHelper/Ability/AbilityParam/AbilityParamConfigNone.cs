using System;
using GAS.RuntimeWithECS;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigNone: AbilityParamConfigBase<AbilityParamNone>
    {
        public override IAbilityParam GetConfig()
        {
            return AbilityParamNone.None;
        }
    }
}