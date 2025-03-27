using System;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [Serializable]
    public class ConfAssetCueOnExecution: BaseGameplayEffectComponentConfigAsset
    {
        // TODO
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfCueOnExecution();
        }
    }
}