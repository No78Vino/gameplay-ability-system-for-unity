using System.Collections.Generic;
using GAS.Runtime.Effects.Execution;
using GAS.Runtime.Effects.Modifier;
using GAS.Runtime.Component;
using GAS.Runtime.Cue;

namespace GAS.Runtime.Effects
{
    public enum EffectsDurationPolicy
    {
        None,
        Instant,
        Infinite,
        Duration
    }
    
    public readonly struct GameplayEffect
    {
        public readonly EffectsDurationPolicy DurationPolicy;
        public readonly float Duration; // -1 represents infinite duration
        public readonly float Period;
        public readonly GameplayEffectTagContainer TagContainer;
        
        // Cues
        readonly List<GameplayCue> CueOnExecute;
        readonly List<GameplayCue> CueOnRemove;
        readonly List<GameplayCue> CueOnAdd;

        public readonly GameplayEffectModifier[] Modifiers;
        public readonly ExecutionCalculation[] Executions;
        
        public GameplayEffectSpec CreateSpec(
            AbilitySystemComponent creator, 
            AbilitySystemComponent owner,
            float level = 1)
        {
            return new GameplayEffectSpec(this, creator, owner, level);
        }
        
        public void TriggerCueOnAdd()
        {
            if (CueOnAdd.Count > 0)
                CueOnAdd.ForEach(cue => cue.Trigger());
        }

        public void TriggerCueOnExecute()
        {
            if (CueOnExecute.Count > 0)
                CueOnExecute.ForEach(cue => cue.Trigger());
        }

        public void TriggerCueOnRemove()
        {
            if (CueOnRemove.Count > 0)
                CueOnRemove.ForEach(cue => cue.Trigger());
        }

        public bool CanApplyTo(AbilitySystemComponent target)
        {
            return target.HasAllTags(TagContainer.ApplicationRequiredTags);
        }
        
        public bool NULL => DurationPolicy == EffectsDurationPolicy.None;
    }
}