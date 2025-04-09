using System.Collections.Generic;
using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.GameplayEffect
{
    [CreateAssetMenu(fileName = "EffectConfigAsset", menuName = "EX-GAS/Effect", order = 0)]
    public class GameplayEffectConfigAsset : ScriptableObject
    {
        [HideLabel]
        public GameplayEffectConfigBase config;

        public GameplayEffectConfig GetConfig()
        {
            var configs = new List<GameplayEffectComponentConfig>();
            foreach (var cfgType in config.configTypes)
            {
                var cfg = config.GetConfigAsset(cfgType);
                if (cfg != null) configs.Add(cfg.GetConfig());
            }

            return new GameplayEffectConfig(configs.ToArray());
        }
    }
}