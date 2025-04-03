using System;
using GAS.RuntimeWithECS.Ability.Component;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigVector2: AbilityParamConfigBase<AbilityParamVector2>
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamVector2(value);
        }
        
        [LabelText("值")]
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public UnityEngine.Vector2 value;
    }
}