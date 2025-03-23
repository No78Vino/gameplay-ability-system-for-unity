using System;
using GAS.RuntimeWithECS.Ability.Component;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigVector3: AbilityParamConfigBase
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamVector3(value);
        }
        
        public UnityEngine.Vector3 value;
    }
}