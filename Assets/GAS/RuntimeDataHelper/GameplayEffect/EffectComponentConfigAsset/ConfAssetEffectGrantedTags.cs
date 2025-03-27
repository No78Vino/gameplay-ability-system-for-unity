using System.Collections.Generic;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [System.Serializable]
    public class ConfAssetEffectGrantedTags: BaseGameplayEffectComponentConfigAsset
    {
        [TabGroup("AbilityAssetTags","效果生效时，额外获得的标签",SdfIconType.TagsFill)]
        [LabelText("标签")]
        [SerializeField] 
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> tags = new();
        
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfEffectGrantedTags()
            {
                tags = tags.ToArray()
            };
        }
    }
}