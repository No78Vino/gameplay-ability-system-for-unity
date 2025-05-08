using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Cue.Component;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
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
        public List<InstantCueSetting> cues = new();

        // TODO
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfCueOnAdd();
        }
    }
}