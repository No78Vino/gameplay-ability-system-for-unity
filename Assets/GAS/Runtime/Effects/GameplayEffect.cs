using GAS.Runtime.Component;
using GAS.Runtime.Cue;
using GAS.Runtime.Effects.Execution;
using GAS.Runtime.Effects.Modifier;

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
        private readonly GameplayCue[] CueOnExecute;
        private readonly GameplayCue[] CueOnRemove;
        private readonly GameplayCue[] CueOnAdd;

        public readonly GameplayEffectModifier[] Modifiers;
        public readonly ExecutionCalculation[] Executions;

        public GameplayEffectSpec CreateSpec(
            AbilitySystemComponent creator,
            AbilitySystemComponent owner,
            float level = 1)
        {
            return new GameplayEffectSpec(this, creator, owner, level);
        }

        public GameplayEffect(GameplayEffectAsset asset)
        {
            DurationPolicy = asset.DurationPolicy;
            Duration = asset.Duration;
            Period = asset.Period;
            TagContainer = new GameplayEffectTagContainer(
                asset.AssetTags,
                asset.GrantedTags,
                asset.ApplicationRequiredTags,
                asset.OngoingRequiredTags,
                asset.RemoveGameplayEffectsWithTags);

            CueOnExecute = asset.CueOnExecute;
            CueOnRemove = asset.CueOnRemove;
            CueOnAdd = asset.CueOnAdd;
            Modifiers = asset.Modifiers;
            Executions = asset.Executions;
        }

        public GameplayEffect(
            EffectsDurationPolicy durationPolicy,
            float duration,
            float period,
            GameplayEffectTagContainer tagContainer,
            GameplayCue[] cueOnExecute,
            GameplayCue[] cueOnRemove,
            GameplayCue[] cueOnAdd,
            GameplayEffectModifier[] modifiers,
            ExecutionCalculation[] executions)
        {
            DurationPolicy = durationPolicy;
            Duration = duration;
            Period = period;
            TagContainer = tagContainer;
            CueOnExecute = cueOnExecute;
            CueOnRemove = cueOnRemove;
            CueOnAdd = cueOnAdd;
            Modifiers = modifiers;
            Executions = executions;
        }

        public void TriggerCueOnAdd()
        {
            if (CueOnAdd.Length <= 0) return;
            foreach (var cue in CueOnAdd) cue.Trigger();
        }

        public void TriggerCueOnExecute()
        {
            if (CueOnExecute.Length <= 0) return;
            foreach (var cue in CueOnExecute) cue.Trigger();
        }

        public void TriggerCueOnRemove()
        {
            if (CueOnRemove.Length <= 0) return;
            foreach (var cue in CueOnRemove) cue.Trigger();
        }

        public bool CanApplyTo(AbilitySystemComponent target)
        {
            return target.HasAllTags(TagContainer.ApplicationRequiredTags);
        }

        public bool NULL => DurationPolicy == EffectsDurationPolicy.None;
    }
}