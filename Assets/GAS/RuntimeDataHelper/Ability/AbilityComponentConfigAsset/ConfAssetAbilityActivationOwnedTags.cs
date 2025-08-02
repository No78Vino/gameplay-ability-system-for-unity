using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Static;
using GAS.RuntimeWithECS.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetAbilityActivationOwnedTags : BaseGameplayAbilityComponentConfigAsset
    {
        [TabGroup(
            "AbilityActivationOwnedTags",
            "能力激活时额外持有标签",
            SdfIconType.TagsFill, TextColor = "#45B188", Order = 1)]
        [LabelText("标签")]
        [SerializeField] 
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> tags = new();

        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfAbilityActivationOwnedTags
            {
                tags = tags.ToArray()
            };
        }
    }
}