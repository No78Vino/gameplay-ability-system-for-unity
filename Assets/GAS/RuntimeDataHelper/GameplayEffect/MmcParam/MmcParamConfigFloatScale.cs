using System;
using GAS.RuntimeWithECS.Modifier.CommonUsage;
using GAS.RuntimeWithECS.Modifier.MmcParameter;

namespace GAS.RuntimeDataHelper.GameplayEffect.MmcParam
{
    [Serializable]
    public class MmcParamConfigFloatScale : MmcParamConfigBase<MmcParaFloatScale>
    {
        public float k;
        public float b;

        public override IMmcParameter GetConfig()
        {
            return new MmcParaFloatScale(k, b);
        }
    }
}