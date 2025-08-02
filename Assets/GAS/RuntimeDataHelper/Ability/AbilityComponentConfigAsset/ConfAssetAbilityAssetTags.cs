using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Static;
using GAS.RuntimeWithECS.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetAbilityAssetTags:BaseGameplayAbilityComponentConfigAsset
    {
        [TabGroup("AbilityAssetTags","能力描述标签",SdfIconType.TagsFill)]
        [LabelText("标签")]
        [SerializeField] 
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> tags = new();
        
        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfAbilityAssetTags
            {
                tags = tags.ToArray()
            };
        }
    }
}