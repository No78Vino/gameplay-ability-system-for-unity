using System;
using GAS.RuntimeWithECS.GameplayEffect;

namespace GAS.RuntimeDataHelper.GameplayEffect
{
    [Serializable]
    public abstract class BaseGameplayEffectComponentConfigAsset
    {
        public abstract GameplayEffectComponentConfig GetConfig();
    }
}