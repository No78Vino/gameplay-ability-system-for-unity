using System;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.Modifier;
using GAS.RuntimeWithECS.Modifier.CommonUsage;

namespace GAS.RuntimeDataHelper.GameplayEffect.MmcParam
{
    [Serializable]
    public abstract class MmcParamConfigBase
    {
        protected ConfModifiers ConfAsset;

        public virtual void SetConfAssetMmc(ConfModifiers config)
        {
            ConfAsset = config;
        }

        public virtual void OnAbilityParamValueChange()
        {
            ConfAsset?.TriggerOnValueChanged();
        }

        public abstract IMmcParameter GetConfig();
    }
    
    [Serializable]
    public abstract class MmcParamConfigBase<T> : MmcParamConfigBase where T : IMmcParameter
    {
    }
}