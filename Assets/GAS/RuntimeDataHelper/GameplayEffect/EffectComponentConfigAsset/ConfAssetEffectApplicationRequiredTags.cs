using System.Collections.Generic;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.RuntimeDataHelper.GameplayEffect.EffectComponentConfigAsset
{
    [System.Serializable]
    public class ConfAssetEffectApplicationRequiredTags: BaseGameplayEffectComponentConfigAsset
    {
        [TabGroup("AbilityAssetTags","效果应用需求标签",SdfIconType.TagsFill)]
        [LabelText("标签")]
        [SerializeField] 
        [ListDrawerSettings]
        [ValueDropdown("@EXEditorHelper.GameplayTagCodeChoices", 
            IsUniqueList = true, 
            HideChildProperties = true)]
        public List<int> tags = new();
        
        public override GameplayEffectComponentConfig GetConfig()
        {
            return new ConfApplicationRequiredTags()
            {
                tags = tags.ToArray()
            };
        }
    }
}