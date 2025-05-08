using System;
using GAS.Runtime;

namespace GAS.Editor
{
    [Serializable]
    public abstract class CueParamConfigBase
    {
        protected ICueSettingConfig setting;

        public virtual void SetConfAssetCue(ICueSettingConfig config)
        {
            setting = config;
        }

        public virtual void OnParamValueChange()
        {
            setting?.TriggerOnValueChanged();
        }

        public abstract ICueParameter GetConfig();
    }
    
    [Serializable]
    public abstract class CueParamConfigBase<T> : CueParamConfigBase where T : ICueParameter
    {
    }
}