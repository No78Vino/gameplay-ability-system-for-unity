using GAS.RuntimeWithECS.GameplayEffect;
using GAS.Runtime;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [System.Serializable]
    public class ConfAssetApplicationCondition: BaseGameplayEffectComponentConfigAsset
    {
        // TODO
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfApplicationCondition();
        }
    }
}