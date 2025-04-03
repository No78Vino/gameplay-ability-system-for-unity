using System;
using GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset;
using GAS.RuntimeWithECS.Ability.Component;

namespace GAS.RuntimeDataHelper.Ability.AbilityParam
{
    [Serializable]
    public abstract class AbilityParamConfigBase
    {
        protected MCConfAssetAbilityLogic ConfAsset;

        public virtual void SetConfAssetAbilityLogic(MCConfAssetAbilityLogic config)
        {
            ConfAsset = config;
        }

        public virtual void OnAbilityParamValueChange()
        {
            ConfAsset?.TriggerOnValueChanged();
        }

        public abstract AbilityParamBase GetConfig();
    }

    [Serializable]
    public abstract class AbilityParamConfigBase<T> : AbilityParamConfigBase where T : AbilityParamBase
    {
    }
}