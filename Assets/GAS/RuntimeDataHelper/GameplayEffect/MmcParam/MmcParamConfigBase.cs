using System;
using GAS.RuntimeWithECS;

namespace GAS.RuntimeDataHelper.GameplayEffect.MmcParam
{
    [Serializable]
    public abstract class MmcParamConfigBase
    {
        protected MMCSettingConfig setting;

        public virtual void SetConfAssetMmc(MMCSettingConfig config)
        {
            setting = config;
        }

        public virtual void OnAbilityParamValueChange()
        {
            setting?.TriggerOnValueChanged();
        }

        public abstract IMmcParameter GetConfig();
    }
    
    [Serializable]
    public abstract class MmcParamConfigBase<T> : MmcParamConfigBase where T : IMmcParameter
    {
    }
}