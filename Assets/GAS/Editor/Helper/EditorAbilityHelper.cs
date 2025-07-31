using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    public enum AbilityEditComponent
    {
        [LabelText("消耗[GE]")]
        Cost,
        
        [LabelText("冷却[GE]")]
        Cooldown,
        
        [LabelText("描述标签")]
        AssetTags,
        
        [LabelText("拥有【任意】Tag的Ability会被取消")]
        CancelAbilityWithTags,
        
        [LabelText("拥有【任意】Tag的Ability会被阻止")]
        BlockAbilityWithTags,
        
        [LabelText("激活后获得的Tag")]
        ActivationOwnedTags,
        
        [LabelText("激活需要的Tag")]
        ActivationRequiredTags,
        
        [LabelText("阻止激活的Tag")]
        ActivationBlockedTags,
        
        [LabelText("技能逻辑")]
        AbilityLogic
    }
    
    public static class EditorAbilityHelper
    {
        public static IEnumerable<AbilityEditComponent> ComponentTypes()
        {
            return new[]
            {
                AbilityEditComponent.Cost,
                AbilityEditComponent.Cooldown,
                AbilityEditComponent.AssetTags,
                AbilityEditComponent.CancelAbilityWithTags,
                AbilityEditComponent.BlockAbilityWithTags,
                AbilityEditComponent.ActivationOwnedTags,
                AbilityEditComponent.ActivationRequiredTags,
                AbilityEditComponent.ActivationBlockedTags,
                AbilityEditComponent.AbilityLogic
            };
        }
    }
}