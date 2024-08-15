using System;

namespace GAS.Runtime
{
    public class InstantGameplayEffectData : IGameplayEffectData
    {
        private string Name { get; }

        public GameplayEffectSnapshotPolicy SnapshotPolicy { get; set; } = GameplayEffectSnapshotPolicy.Specified;

        public GameplayTag[] ApplicationRequiredTags { get; set; } = Array.Empty<GameplayTag>();
        public GameplayTag[] ApplicationImmunityTags { get; set; } = Array.Empty<GameplayTag>();
        public GameplayTag[] RemoveGameplayEffectsWithTags { get; set; } = Array.Empty<GameplayTag>();
        public GameplayCueInstant[] CueOnExecute { get; set; } = Array.Empty<GameplayCueInstant>();
        public GameplayEffectModifier[] Modifiers { get; set; } = Array.Empty<GameplayEffectModifier>();
        public GameplayEffectSpecifiedSnapshotConfig[] SpecifiedSnapshotConfigs { get; set; } = Array.Empty<GameplayEffectSpecifiedSnapshotConfig>();

        public InstantGameplayEffectData(string name) => Name = name;

        public string GetDisplayName() => Name;

        public virtual EffectsDurationPolicy GetDurationPolicy() => EffectsDurationPolicy.Instant;

        public virtual float GetDuration() => -1;

        public virtual float GetPeriod() => 0;

        public virtual IGameplayEffectData GetPeriodExecution() => null;

        public GameplayEffectSnapshotPolicy GetSnapshotPolicy() => SnapshotPolicy;

        public GameplayEffectSpecifiedSnapshotConfig[] GetSpecifiedSnapshotConfigs() => SpecifiedSnapshotConfigs;

        public virtual GameplayTag[] GetAssetTags() => Array.Empty<GameplayTag>();

        public virtual GameplayTag[] GetGrantedTags() => Array.Empty<GameplayTag>();

        public GameplayTag[] GetRemoveGameplayEffectsWithTags() => RemoveGameplayEffectsWithTags;

        public GameplayTag[] GetApplicationRequiredTags() => ApplicationRequiredTags;

        public GameplayTag[] GetApplicationImmunityTags() => ApplicationImmunityTags;

        public virtual GameplayTag[] GetOngoingRequiredTags() => Array.Empty<GameplayTag>();

        public GameplayCueInstant[] GetCueOnExecute() => CueOnExecute;

        public virtual GameplayCueInstant[] GetCueOnRemove() => Array.Empty<GameplayCueInstant>();

        public virtual GameplayCueInstant[] GetCueOnAdd() => Array.Empty<GameplayCueInstant>();

        public virtual GameplayCueInstant[] GetCueOnActivate() => Array.Empty<GameplayCueInstant>();

        public virtual GameplayCueInstant[] GetCueOnDeactivate() => Array.Empty<GameplayCueInstant>();

        public virtual GameplayCueDurational[] GetCueDurational() => Array.Empty<GameplayCueDurational>();

        public GameplayEffectModifier[] GetModifiers() => Modifiers;

        public virtual ExecutionCalculation[] GetExecutions() => Array.Empty<ExecutionCalculation>();

        public virtual GrantedAbilityConfig[] GetGrantedAbilities() => Array.Empty<GrantedAbilityConfig>();

        public virtual GameplayEffectStacking GetStacking() => GameplayEffectStacking.None;
    }

    public class InfiniteGameplayEffectData : InstantGameplayEffectData
    {
        public float Period { get; }

        public IGameplayEffectData PeriodExecution { get; set; } = null;

        public GameplayTag[] AssetTags { get; set; } = Array.Empty<GameplayTag>();
        public GameplayTag[] GrantedTags { get; set; } = Array.Empty<GameplayTag>();
        public GameplayTag[] OngoingRequiredTags { get; set; } = Array.Empty<GameplayTag>();

        public GameplayCueInstant[] CueOnRemove { get; set; } = Array.Empty<GameplayCueInstant>();
        public GameplayCueInstant[] CueOnAdd { get; set; } = Array.Empty<GameplayCueInstant>();
        public GameplayCueInstant[] CueOnActivate { get; set; } = Array.Empty<GameplayCueInstant>();
        public GameplayCueInstant[] CueOnDeactivate { get; set; } = Array.Empty<GameplayCueInstant>();
        public GameplayCueDurational[] CueDurational { get; set; } = Array.Empty<GameplayCueDurational>();
        public ExecutionCalculation[] Executions { get; set; } = Array.Empty<ExecutionCalculation>();
        public GrantedAbilityConfig[] GrantedAbilities { get; set; } = Array.Empty<GrantedAbilityConfig>();
        public GameplayEffectStacking Stacking { get; set; } = GameplayEffectStacking.None;

        public InfiniteGameplayEffectData(string name, float period) : base(name) => Period = period;

        public override EffectsDurationPolicy GetDurationPolicy() => EffectsDurationPolicy.Infinite;

        public override float GetPeriod() => Period;

        public override IGameplayEffectData GetPeriodExecution() => PeriodExecution;

        public override GameplayTag[] GetAssetTags() => AssetTags;

        public override GameplayTag[] GetGrantedTags() => GrantedTags;

        public override GameplayTag[] GetOngoingRequiredTags() => OngoingRequiredTags;

        public override GameplayCueInstant[] GetCueOnRemove() => CueOnRemove;

        public override GameplayCueInstant[] GetCueOnAdd() => CueOnAdd;

        public override GameplayCueInstant[] GetCueOnActivate() => CueOnActivate;

        public override GameplayCueInstant[] GetCueOnDeactivate() => CueOnDeactivate;

        public override GameplayCueDurational[] GetCueDurational() => CueDurational;

        public override ExecutionCalculation[] GetExecutions() => Executions;

        public override GrantedAbilityConfig[] GetGrantedAbilities() => GrantedAbilities;

        public override GameplayEffectStacking GetStacking() => Stacking;
    }

    public class DurationalGameplayEffectData : InfiniteGameplayEffectData
    {
        public float Duration { get; }

        public DurationalGameplayEffectData(string name, float period, float duration) : base(name, period) => Duration = duration;

        public override EffectsDurationPolicy GetDurationPolicy() => EffectsDurationPolicy.Duration;

        public override float GetDuration() => Duration;
    }
}