using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [Serializable]
    public class ConfAssetOngoingRequiredTags: BaseGameplayEffectComponentConfigAsset
    {
        [TabGroup("AbilityAssetTags","该效果持续生效所需标签",SdfIconType.TagsFill)]
        [LabelText("标签")]
        [SerializeField] 
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> tags = new();
        
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfOngoingRequiredTags()
            {
                tags = tags.ToArray()
            };
        }
    }
}