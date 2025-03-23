using System;
using GAS.RuntimeWithECS.Ability.Component;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public class AbilityParamConfigInt: AbilityParamConfigBase<AbilityParamInt>
    {
        public override AbilityParamBase GetConfig()
        {
            return new AbilityParamInt(value);
        }
        
        [LabelText("值")]
        public int value;
    }
}