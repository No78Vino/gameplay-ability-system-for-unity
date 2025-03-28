using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [Serializable]
    public class ConfAssetRemoveEffectWithTags: BaseGameplayEffectComponentConfigAsset
    {
        [TabGroup("RemoveEffectWithTags","该效果会移除持有以下标签的其它效果",SdfIconType.TagsFill)]
        [LabelText("标签")]
        [SerializeField] 
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> tags = new();
        
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfRemoveEffectWithTags()
            {
                tags = tags.ToArray()
            };
        }
    }
}