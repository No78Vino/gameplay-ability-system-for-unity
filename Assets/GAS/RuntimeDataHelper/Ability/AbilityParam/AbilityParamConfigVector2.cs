using System;
using GAS.RuntimeWithECS.Ability.Component;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigVector2: AbilityParamConfigBase
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamVector2(value);
        }
        
        public UnityEngine.Vector2 value;
    }
}