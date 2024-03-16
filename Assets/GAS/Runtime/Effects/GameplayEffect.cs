
namespace GAS.Runtime
{
    public enum EffectsDurationPolicy
    {
        Instant = 1,
        Infinite,
        Duration
    }

    public class GameplayEffect
    {
        public readonly string GameplayEffectName;
        public readonly EffectsDurationPolicy DurationPolicy;
        public readonly float Duration; // -1 represents infinite duration
        public readonly float Period;
        public readonly GameplayEffect PeriodExecution;
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
            GameplayEffectName = asset.name;
            DurationPolicy = asset.DurationPolicy;
            Duration = asset.Duration;
            Period = asset.Period;
            TagContainer = new GameplayEffectTagContainer(
                asset.AssetTags,
                asset.GrantedTags,
                asset.ApplicationRequiredTags,
                asset.OngoingRequiredTags,
                asset.RemoveGameplayEffectsWithTags,
                asset.ApplicationImmunityTags);
            PeriodExecution = asset.PeriodExecution != null ? new GameplayEffect(asset.PeriodExecution) : null;
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
            GameplayEffect periodExecution,
            GameplayEffectTagContainer tagContainer,
            GameplayCueInstant[] cueOnExecute,
            GameplayCueInstant[] cueOnAdd,
            GameplayCueInstant[] cueOnRemove,
            GameplayCueInstant[] cueOnActivate,
            GameplayCueInstant[] cueOnDeactivate,
            GameplayCueDurational[] cueDurational,
            GameplayEffectModifier[] modifiers,
            ExecutionCalculation[] executions)
        {
            GameplayEffectName = null;
            DurationPolicy = durationPolicy;
            Duration = duration;
            Period = period;
            PeriodExecution = periodExecution;
            TagContainer = tagContainer;
            CueOnExecute = cueOnExecute;
            CueOnRemove = cueOnRemove;
            CueOnAdd = cueOnAdd;
            CueOnActivate = cueOnActivate;
            CueOnDeactivate = cueOnDeactivate;
            CueDurational = cueDurational;
            Modifiers = modifiers;
            Executions = executions;
        }

        public bool CanApplyTo(IAbilitySystemComponent target)
        {
            return target.HasAllTags(TagContainer.ApplicationRequiredTags);
        }
    }
}