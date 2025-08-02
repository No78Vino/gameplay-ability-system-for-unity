using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Static;
using GAS.RuntimeWithECS.ComponentConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.Ability.AbilityComponentConfigAsset
{
    [Serializable]
    public class ConfAssetCancelAbilityTags : BaseGameplayAbilityComponentConfigAsset
    {
        [TabGroup("BlockAbilityTags","取消含有以下标签的能力",SdfIconType.TagsFill, TextColor = "#99B188")]
        [LabelText("标签")]
        [SerializeField] 
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> tags = new();

        public override GameplayAbilityComponentConfig GetConfig()
        {
            return new ConfCancelAbilityTags()
            {
                tags = tags.ToArray()
            };
        }
    }
}