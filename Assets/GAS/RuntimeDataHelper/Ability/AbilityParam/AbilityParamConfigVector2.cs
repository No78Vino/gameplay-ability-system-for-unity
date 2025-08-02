using System;
using GAS.RuntimeWithECS;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigVector2: AbilityParamConfigBase<AbilityParamVector2>
    {
        public override IAbilityParam GetConfig()
        {
            return new AbilityParamVector2(value);
        }
        
        [LabelText("值")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public UnityEngine.Vector2 value;
    }
}