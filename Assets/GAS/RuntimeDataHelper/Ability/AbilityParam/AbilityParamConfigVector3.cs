using System;
using GAS.RuntimeWithECS;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigVector3: AbilityParamConfigBase<AbilityParamVector3>
    {
        public override IAbilityParam GetConfig()
        {
            return new AbilityParamVector3(value);
        }
        
        [LabelText("值")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public UnityEngine.Vector3 value;
    }
}