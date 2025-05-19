using System;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.Runtime;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [Serializable]
    public class ConfAssetStacking : BaseGameplayEffectComponentConfigAsset
    {
        public EffectStackType StackType;
        public int StackingCode;
        public int LimitCount;

        public EffectDurationRefreshPolicy EffectDurationRefreshPolicy;
        public EffectPeriodResetPolicy EffectPeriodResetPolicy;
        public EffectExpirationPolicy expirationPolicy;

        public bool denyOverflowApplication;
        public bool clearStackOnOverflow;
        public GameplayEffectConfigAsset[] overflowEffects;

        public override GameplayEffectComponentConfig GetConfig()
        {
            var effects = new GameplayEffectConfig[overflowEffects.Length];
            for (var i = 0; i < overflowEffects.Length; i++) 
                effects[i] = overflowEffects[i].GetConfig();
            return new ConfStacking
            {
                StackType = StackType,
                StackingCode = StackingCode,
                LimitCount = LimitCount,
                EffectDurationRefreshPolicy = EffectDurationRefreshPolicy,
                EffectPeriodResetPolicy = EffectPeriodResetPolicy,
                EffectExpirationPolicy = expirationPolicy,
                denyOverflowApplication = denyOverflowApplication,
                clearStackOnOverflow = clearStackOnOverflow,
                overflowEffects = effects
            };
        }
    }
}