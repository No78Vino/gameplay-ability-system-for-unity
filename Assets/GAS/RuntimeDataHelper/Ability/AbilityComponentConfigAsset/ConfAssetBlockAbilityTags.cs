using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Ability.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetBlockAbilityTags : BaseGameplayAbilityComponentConfigAsset
    {
        [TabGroup("BlockAbilityTags","暂时打断含有以下标签的能力",SdfIconType.TagsFill, TextColor = "#99B188")]
        [LabelText("标签")]
        [SerializeField] 
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> tags = new();

        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfBlockAbilityTags
            {
                tags = tags.ToArray()
            };
        }
    }
}