using System;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.Runtime;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [Serializable]
    public class ConfAssetPeriod: BaseGameplayEffectComponentConfigAsset
    {
        public int Period;
        public GameplayEffectConfigAsset[] GameplayEffectSettings;

        public override GameplayEffectComponentConfig GetConfig()
        {
            var gameplayEffectSettings = new GameplayEffectComponentConfig[GameplayEffectSettings.Length][];
            for (var i = 0; i < GameplayEffectSettings.Length; i++)
            {
                gameplayEffectSettings[i] = GameplayEffectSettings[i].GetConfig().ComponentConfigs;
            }
            return new ConfPeriod()
            {
                Period = Period,
                GameplayEffectSettings = gameplayEffectSettings
            };
        }
    }
}