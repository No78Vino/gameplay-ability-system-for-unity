using System;
using GAS.RuntimeWithECS.Modifier.CommonUsage;
using GAS.RuntimeWithECS.Modifier.MmcParameter;
using Sirenix.OdinInspector;

namespace GAS.RuntimeDataHelper.GameplayEffect.MmcParam
{
    [Serializable]
    public class MmcParamConfigFloatScale : MmcParamConfigBase<MmcParaFloatScale>
    {
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public float k;
        
        [OnValueChanged(nameof(OnAbilityParamValueChange))]
        public float b;

        public override IMmcParameter GetConfig()
        {
            return new MmcParaFloatScale(k, b);
        }
    }
}