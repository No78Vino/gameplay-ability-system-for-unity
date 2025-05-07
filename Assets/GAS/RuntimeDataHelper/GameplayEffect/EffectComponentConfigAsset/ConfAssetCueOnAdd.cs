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
        [TabGroup("Modifiers","添加时Cue",SdfIconType.TagsFill)]
        [LabelText("")]
        [SerializeField] 
        [ListDrawerSettings]
        public List<InstantCueSetting> cues = new();
        
        // public override GameplayEffectComponentConfig GetConfig()
        // {
        //     return new MCConfModifiers()
        //     {
        //         modifierSettings = modifiers.ToArray()
        //     };
        // }
        
        // TODO
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfCueOnAdd();
        }
    }
}