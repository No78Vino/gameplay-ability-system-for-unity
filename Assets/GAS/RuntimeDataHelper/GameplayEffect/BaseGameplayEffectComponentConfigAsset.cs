using System;
using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect;

namespace GAS.RuntimeDataHelper.GameplayEffect
{
    [Serializable]
    public abstract class BaseGameplayEffectComponentConfigAsset
    {
        public abstract GameplayEffectComponentConfig GetConfig();

        public void SetOwnAsset(GameplayEffectConfigBase config)
        {
            _config = config;
        }
        
        protected GameplayEffectConfigBase _config;
        
        protected virtual void OnValueChanged()
        {
            //if (_config) EditorUtility.SetDirty(Asset);
        }
    }
}