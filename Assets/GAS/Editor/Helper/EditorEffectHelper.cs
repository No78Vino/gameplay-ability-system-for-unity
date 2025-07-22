using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    public enum EffectEditComponent
    {
        [LabelText("描述标签")]
        AssetTags,
        
        [LabelText("获得标签")]
        GrantedTags,
        
        [LabelText("应用需求标签")]
        ApplicationRequiredTags,
        
        [LabelText("持续需求标签")]
        OngoingRequiredTags,
        
        [LabelText("移除持有标签的buff")]
        RemoveGameplayEffectsWithTags,
        
        [LabelText("被免疫的标签")]
        ImmunityTags,
        
        [LabelText("持续时间")]
        Duration,
        
        [LabelText("间隔执行")]
        Period,
        
        [LabelText("修改器")]
        Modifiers,
        
        [LabelText("应用时触发的Cue")]
        CueOnApply,
        
        [LabelText("帧更新的Cue")]
        CueOnTick,
        
        [LabelText("添加时触发的Cue")]
        CueOnAdd,
        
        [LabelText("移除时触发的Cue")]
        CueOnRemove,	
        
        [LabelText("激活时触发的Cue")]
        CueOnActivate,
        
        [LabelText("失活时触发的Cue")]
        CueOnDeactivate,
        
        [LabelText("获取技能")]
        GrantedAbility,
        
        [LabelText("buff堆叠")]
        Stacking
    }
    
    public static class EditorEffectHelper
    {
        public static IEnumerable<EffectEditComponent> ComponentTypes()
        {
            return new[]
            {
                EffectEditComponent.AssetTags,
                EffectEditComponent.GrantedTags,
                EffectEditComponent.ApplicationRequiredTags,
                EffectEditComponent.OngoingRequiredTags,
                EffectEditComponent.RemoveGameplayEffectsWithTags,
                EffectEditComponent.ImmunityTags,
                EffectEditComponent.Duration,
                EffectEditComponent.Period,
                EffectEditComponent.Modifiers,
                EffectEditComponent.CueOnApply,
                EffectEditComponent.CueOnTick,
                EffectEditComponent.CueOnAdd,
                EffectEditComponent.CueOnRemove,
                EffectEditComponent.CueOnActivate,
                EffectEditComponent.CueOnDeactivate,
                EffectEditComponent.GrantedAbility,
                EffectEditComponent.Stacking
            };
        }
    }
}