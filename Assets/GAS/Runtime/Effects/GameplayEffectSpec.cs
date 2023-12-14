using System.Collections.Generic;
using GAS.General;
using Unity.Mathematics;

namespace GAS.Runtime.Effects
{
    public class GameplayEffectSpec
    {
        private GameplayEffectSpec(GameplayEffect gameplayEffect, AbilitySystemComponent.AbilitySystemComponent source,
            float level = 1)
        {
            GameplayEffect = gameplayEffect;
            Source = source;
            Targets = new List<AbilitySystemComponent.AbilitySystemComponent>();
            Level = level;

            if (gameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant)
                PeriodTicker = new GameplayEffectPeriodTicker();
        }

        public GameplayEffect GameplayEffect { get; }

        public long ActivationTime { get; private set; }

        public float Duration => GameplayEffect.Duration;

        public float Level { get; private set; }

        public AbilitySystemComponent.AbilitySystemComponent Source { get; private set; }

        public List<AbilitySystemComponent.AbilitySystemComponent> Targets { get; private set; }

        public bool IsActive { get; private set; }

        public GameplayEffectPeriodTicker PeriodTicker { get; private set; }

        public float DurationRemaining()
        {
            if (GameplayEffect.DurationPolicy == EffectsDurationPolicy.Infinite)
                return -1;

            return math.max(0, Duration - (GASTimer.Timestamp() - ActivationTime) / 1000f);
        }

        public void AddTarget(AbilitySystemComponent.AbilitySystemComponent target)
        {
            if (Targets.Contains(target)) return;
            Targets.Add(target);
        }
        
        public bool RemoveTarget(AbilitySystemComponent.AbilitySystemComponent target)
        {
            return Targets.Remove(target);
        }

        // public GameplayEffectSpec TickPeriodic(float deltaTime, out bool executePeriodicTick)
        // {
        //     this.TimeUntilPeriodTick -= deltaTime;
        //     executePeriodicTick = false;
        //     if (this.TimeUntilPeriodTick <= 0)
        //     {
        //         this.TimeUntilPeriodTick = GameplayEffect.GetPeriod().Period;
        //
        //         // Check to make sure period is valid, otherwise we'd just end up executing every frame
        //         if (GameplayEffect.GetPeriod().Period > 0) executePeriodicTick = true;
        //     }
        //
        //     return this;
        // }

        public void SetLevel(float level)
        {
            Level = level;
        }

        public void Activate()
        {
            IsActive = true;
            ActivationTime = GASTimer.Timestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
        }
        
        public bool CanApplyToTarget(AbilitySystemComponent.AbilitySystemComponent target )
        {
            return target.HasAllTags(GameplayEffect.NecessaryTags) &&
                   !target.HasAnyTags(GameplayEffect.RejectionTags);
        }
    }
}