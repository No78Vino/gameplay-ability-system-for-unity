//using UnityEngine.Profiling;

using System.Collections.Generic;

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

        // Modifiers
        public readonly GameplayEffectModifier[] Modifiers;
        public readonly ExecutionCalculation[] Executions; // TODO: this should be a list of execution calculations

        // Granted Ability
        public readonly GrantedAbilityFromEffect[] GrantedAbilities;

        public GameplayEffectSpec CreateSpec(
            AbilitySystemComponent creator,
            AbilitySystemComponent owner,
            float level = 1)
        {
            //Profiler.BeginSample("[GC Mark] GameplayEffectSpec.CreateSpec()");
            var spec = new GameplayEffectSpec(this, creator, owner, level);
            //Profiler.EndSample();
            return spec;
        }

        public GameplayEffect(IGameplayEffectData data)
        {
            GameplayEffectName = data.GetDisplayName();
            DurationPolicy = data.GetDurationPolicy();
            Duration = data.GetDuration();
            Period = data.GetPeriod();
            TagContainer = new GameplayEffectTagContainer(data);
            PeriodExecution = data.GetPeriodExecution() != null ? new GameplayEffect(data.GetPeriodExecution()) : null;
            CueOnExecute = data.GetCueOnExecute();
            CueOnRemove = data.GetCueOnRemove();
            CueOnAdd = data.GetCueOnAdd();
            CueOnActivate = data.GetCueOnActivate();
            CueOnDeactivate = data.GetCueOnDeactivate();
            CueDurational = data.GetCueDurational();
            Modifiers = data.GetModifiers();
            Executions = data.GetExecutions();
            GrantedAbilities = GetGrantedAbilities(data.GetGrantedAbilities());
        }

        private static GrantedAbilityFromEffect[] GetGrantedAbilities(IEnumerable<GrantedAbilityConfig> grantedAbilities)
        {
            var grantedAbilityList = new List<GrantedAbilityFromEffect>();
            foreach (var grantedAbilityConfig in grantedAbilities)
            {
                if (grantedAbilityConfig.AbilityAsset == null) continue;
                grantedAbilityList.Add(new GrantedAbilityFromEffect(grantedAbilityConfig));
            }

            return grantedAbilityList.ToArray();
        }

        public bool CanApplyTo(IAbilitySystemComponent target)
        {
            return target.HasAllTags(TagContainer.ApplicationRequiredTags);
        }
        
        public bool CanRunning(IAbilitySystemComponent target)
        {
            return target.HasAllTags(TagContainer.OngoingRequiredTags);
        }
        
        public bool IsImmune(IAbilitySystemComponent target)
        {
            return target.HasAnyTags(TagContainer.ApplicationImmunityTags);
        }
    }
}