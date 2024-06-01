using System;

namespace GAS.Runtime
{
    public class InstantGameplayEffectData : IGameplayEffectData
    {
        private string Name { get; }

        public GameplayTag[] ApplicationRequiredTags { get; set; } = Array.Empty<GameplayTag>();
        public GameplayTag[] ApplicationImmunityTags { get; set; } = Array.Empty<GameplayTag>();
        public GameplayTag[] RemoveGameplayEffectsWithTags { get; set; } = Array.Empty<GameplayTag>();
        public GameplayCueInstant[] CueOnExecute { get; set; } = Array.Empty<GameplayCueInstant>();
        public GameplayEffectModifier[] Modifiers { get; set; } = Array.Empty<GameplayEffectModifier>();

        public InstantGameplayEffectData(string name)
        {
            Name = name;
        }

        public string GetDisplayName()
        {
            return Name;
        }

        public virtual EffectsDurationPolicy GetDurationPolicy()
        {
            return EffectsDurationPolicy.Instant;
        }

        public virtual float GetDuration()
        {
            return -1;
        }

        public virtual float GetPeriod()
        {
            return 0;
        }

        public virtual IGameplayEffectData GetPeriodExecution()
        {
            return null;
        }

        public virtual GameplayTag[] GetAssetTags()
        {
            return Array.Empty<GameplayTag>();
        }

        public virtual GameplayTag[] GetGrantedTags()
        {
            return Array.Empty<GameplayTag>();
        }

        public GameplayTag[] GetRemoveGameplayEffectsWithTags()
        {
            return RemoveGameplayEffectsWithTags;
        }

        public GameplayTag[] GetApplicationRequiredTags()
        {
            return ApplicationRequiredTags;
        }

        public GameplayTag[] GetApplicationImmunityTags()
        {
            return ApplicationImmunityTags;
        }

        public virtual GameplayTag[] GetOngoingRequiredTags()
        {
            return Array.Empty<GameplayTag>();
        }

        public GameplayCueInstant[] GetCueOnExecute()
        {
            return CueOnExecute;
        }

        public virtual GameplayCueInstant[] GetCueOnRemove()
        {
            return Array.Empty<GameplayCueInstant>();
        }

        public virtual GameplayCueInstant[] GetCueOnAdd()
        {
            return Array.Empty<GameplayCueInstant>();
        }

        public virtual GameplayCueInstant[] GetCueOnActivate()
        {
            return Array.Empty<GameplayCueInstant>();
        }

        public virtual GameplayCueInstant[] GetCueOnDeactivate()
        {
            return Array.Empty<GameplayCueInstant>();
        }

        public virtual GameplayCueDurational[] GetCueDurational()
        {
            return Array.Empty<GameplayCueDurational>();
        }

        public GameplayEffectModifier[] GetModifiers()
        {
            return Modifiers;
        }

        public virtual ExecutionCalculation[] GetExecutions()
        {
            return Array.Empty<ExecutionCalculation>();
        }

        public virtual GrantedAbilityConfig[] GetGrantedAbilities()
        {
            return Array.Empty<GrantedAbilityConfig>();
        }

        public virtual GameplayEffectStacking GetStacking()
        {
            return GameplayEffectStacking.None;
        }
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

        public InfiniteGameplayEffectData(string name, float period) : base(name)
        {
            Period = period;
        }

        public override EffectsDurationPolicy GetDurationPolicy()
        {
            return EffectsDurationPolicy.Infinite;
        }

        public override float GetPeriod()
        {
            return Period;
        }

        public override IGameplayEffectData GetPeriodExecution()
        {
            return PeriodExecution;
        }

        public override GameplayTag[] GetAssetTags()
        {
            return AssetTags;
        }

        public override GameplayTag[] GetGrantedTags()
        {
            return GrantedTags;
        }

        public override GameplayTag[] GetOngoingRequiredTags()
        {
            return OngoingRequiredTags;
        }

        public override GameplayCueInstant[] GetCueOnRemove()
        {
            return CueOnRemove;
        }

        public override GameplayCueInstant[] GetCueOnAdd()
        {
            return CueOnAdd;
        }

        public override GameplayCueInstant[] GetCueOnActivate()
        {
            return CueOnActivate;
        }

        public override GameplayCueInstant[] GetCueOnDeactivate()
        {
            return CueOnDeactivate;
        }

        public override GameplayCueDurational[] GetCueDurational()
        {
            return CueDurational;
        }

        public override ExecutionCalculation[] GetExecutions()
        {
            return Executions;
        }

        public override GrantedAbilityConfig[] GetGrantedAbilities()
        {
            return GrantedAbilities;
        }

        public override GameplayEffectStacking GetStacking()
        {
            return Stacking;
        }
    }

    public class DurationalGameplayEffectData : InfiniteGameplayEffectData
    {
        public float Duration { get; }

        public DurationalGameplayEffectData(string name, float period, float duration) : base(name, period)
        {
            Duration = duration;
        }

        public override EffectsDurationPolicy GetDurationPolicy()
        {
            return EffectsDurationPolicy.Duration;
        }

        public override float GetDuration()
        {
            return Duration;
        }
    }
}