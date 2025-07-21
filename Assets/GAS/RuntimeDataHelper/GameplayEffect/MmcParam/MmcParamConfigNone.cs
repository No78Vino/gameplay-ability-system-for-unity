using System;
using GAS.RuntimeWithECS;
using GAS.RuntimeWithECS.Modifier.MmcParameter;

namespace GAS.RuntimeDataHelper.GameplayEffect.MmcParam
{
    [Serializable]
    public class MmcParamConfigNone : MmcParamConfigBase<MmcParamNone>
    {
        public override IMmcParameter GetConfig()
        {
            return new MmcParamNone();
        }
    }
}