using System;
using System.Collections.Generic;
using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [Serializable]
    public class ConfAssetCueOnAdd: BaseGameplayEffectComponentConfigAsset
    {
        [TabGroup("Cue","添加时Cue",SdfIconType.MusicNote)]
        [LabelText(" ")]
        [SerializeField] 
        [ListDrawerSettings]
        public List<CueSetting> cues = new();
        
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfCueOnAdd()
            {
            };
        }
    }
}