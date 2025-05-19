using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [Serializable]
    public class MCConfAssetModifiers: BaseGameplayEffectComponentConfigAsset
    {
        [TabGroup("Modifiers","修改器",SdfIconType.TagsFill)]
        [LabelText("")]
        [SerializeField] 
        [ListDrawerSettings]
        public List<ModifierSetting> modifiers = new();
        
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new MCConfModifiers()
            {
                modifierSettings = modifiers.ToArray()
            };
        }
    }
}