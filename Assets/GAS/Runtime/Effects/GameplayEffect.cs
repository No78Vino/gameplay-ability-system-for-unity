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
        public readonly GameplayEffectAsset Asset;
        public readonly EffectsDurationPolicy DurationPolicy;
        public readonly float Duration; // -1 represents infinite duration
        public readonly float Period;
        public readonly GameplayEffectTagContainer TagContainer;

        // Cues
        public readonly GameplayCueInstant[] CueOnExecute;
        public readonly GameplayCueInstant[] CueOnRemove;
        public readonly GameplayCueInstant[] CueOnAdd;
        public readonly GameplayCueInstant[] CueOnActivate;
        public readonly GameplayCueInstant[] CueOnDeactivate;
        public readonly GameplayCueDurational[] CueDurational;

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
            Asset = asset;
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
            CueOnActivate = asset.CueOnActivate;
            CueOnDeactivate = asset.CueOnDeactivate;
            CueDurational = asset.CueDurational;
            Modifiers = asset.Modifiers;
            Executions = asset.Executions;
        }

        public GameplayEffect(
            EffectsDurationPolicy durationPolicy,
            float duration,
            float period,
            GameplayEffectTagContainer tagContainer,
            GameplayCueInstant[] cueOnExecute,
            GameplayCueInstant[] cueOnAdd,
            GameplayCueInstant[] cueOnRemove,
            GameplayCueInstant[] cueOnActivate,
            GameplayCueInstant[] cueOnDeactivate,
            GameplayCueDurational[] cueDurationals,
            GameplayEffectModifier[] modifiers,
            ExecutionCalculation[] executions)
        {
            Asset = null;
            DurationPolicy = durationPolicy;
            Duration = duration;
            Period = period;
            TagContainer = tagContainer;
            CueOnExecute = cueOnExecute;
            CueOnRemove = cueOnRemove;
            CueOnAdd = cueOnAdd;
            CueOnActivate = cueOnActivate;
            CueOnDeactivate = cueOnDeactivate;
            CueDurational = cueDurationals;
            Modifiers = modifiers;
            Executions = executions;
        }

        public bool CanApplyTo(IAbilitySystemComponent target)
        {
            return target.HasAllTags(TagContainer.ApplicationRequiredTags);
        }

        public bool NULL => DurationPolicy == EffectsDurationPolicy.None;
    }
}