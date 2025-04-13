using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
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
        // [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
        //     IsUniqueList = true, 
        //     HideChildProperties = true)]
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