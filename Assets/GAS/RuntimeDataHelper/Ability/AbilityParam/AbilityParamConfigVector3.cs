using System;
using GAS.RuntimeWithECS.Ability.Component;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigVector3: AbilityParamConfigBase<AbilityParamVector3>
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamVector3(value);
        }
        
        [LabelText("值")]
        public UnityEngine.Vector3 value;
    }
}