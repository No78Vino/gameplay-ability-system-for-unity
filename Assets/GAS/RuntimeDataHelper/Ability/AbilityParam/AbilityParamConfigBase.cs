using System;
using GAS.RuntimeWithECS.Ability.Component;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public abstract class AbilityParamConfigBase
    {
        public abstract AbilityParamBase GetConfig();
    }
}