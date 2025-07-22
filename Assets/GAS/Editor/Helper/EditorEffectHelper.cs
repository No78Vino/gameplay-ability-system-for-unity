using System.Collections.Generic;

namespace GAS.Editor
{
    public enum EffectEditComponent
    {
        AssetTags,
        GrantedTags,
        ApplicationRequiredTags,
        OngoingRequiredTags,
        RemoveGameplayEffectsWithTags,
        ImmunityTags,
        Duration,
        Period,
        Modifiers,
        CueOnApply,
        CueOnTick,
        CueOnAdd,
        CueOnRemove,	
        CueOnActivate,
        CueOnDeactivate,
        GrantedAbility,
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