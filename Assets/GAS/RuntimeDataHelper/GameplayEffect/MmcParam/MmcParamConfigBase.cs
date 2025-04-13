using System;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS.Modifier.CommonUsage;

namespace GAS.RuntimeDataHelper.GameplayEffect.MmcParam
{
    [Serializable]
    public abstract class MmcParamConfigBase
    {
        protected MCConfModifiers McConfAsset;

        public virtual void SetConfAssetMmc(MCConfModifiers config)
        {
            McConfAsset = config;
        }

        public virtual void OnAbilityParamValueChange()
        {
            McConfAsset?.TriggerOnValueChanged();
        }

        public abstract IMmcParameter GetConfig();
    }
    
    [Serializable]
    public abstract class MmcParamConfigBase<T> : MmcParamConfigBase where T : IMmcParameter
    {
    }
}