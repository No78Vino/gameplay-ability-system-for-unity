using System.Collections.Generic;
using GAS.General;
using GAS.Runtime.Attribute;
using GAS.Runtime.Effects.Modifier;
using Unity.Mathematics;

namespace GAS.Runtime.Effects
{
    public class GameplayEffectSpec
    {
        private List<GameplayEffectModifier> _modifierStack;
        public GameplayEffectSpec(GameplayEffect gameplayEffect, AbilitySystemComponent.AbilitySystemComponent source,
            float level = 1)
        {
            GameplayEffect = gameplayEffect;
            Source = source;
            Targets = new List<AbilitySystemComponent.AbilitySystemComponent>();
            Level = level;
            
            if (gameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant)
                PeriodTicker = new GameplayEffectPeriodTicker(this);
        }

        public GameplayEffect GameplayEffect { get; }

        public long ActivationTime { get; private set; }

        public float Duration => GameplayEffect.Duration;

        public float Level { get; private set; }

        public AbilitySystemComponent.AbilitySystemComponent Source { get; private set; }

        public List<AbilitySystemComponent.AbilitySystemComponent> Targets { get; private set; }

        public bool IsActive { get; private set; }

        public GameplayEffectPeriodTicker PeriodTicker { get; private set; }
        
        /// <summary>
        /// Snapshotting of Attributes when the GameplayEffect is activated
        /// </summary>
        public List<AttributeBase> SnapshottingAttributes{ get; private set; }

        public float DurationRemaining()
        {
            if (GameplayEffect.DurationPolicy == EffectsDurationPolicy.Infinite)
                return -1;

            return math.max(0, Duration - (GASTimer.Timestamp() - ActivationTime) / 1000f);
        }

        public void AddTarget(AbilitySystemComponent.AbilitySystemComponent target)
        {
            if (!CanApplyToTarget(target)) return;
            if (Targets.Contains(target)) return;
            Targets.Add(target);
        }

        public bool RemoveTarget(AbilitySystemComponent.AbilitySystemComponent target)
        {
            return Targets.Remove(target);
        }

        public void SetLevel(float level)
        {
            Level = level;
        }

        public void Activate()
        {
            IsActive = true;
            ActivationTime = GASTimer.Timestamp();
            
            GameplayEffect.TriggerOnActivation();

            if (GameplayEffect.DurationPolicy == EffectsDurationPolicy.Instant)
                GameplayEffect.TriggerOnExecute();
        }

        public void Deactivate()
        {
            IsActive = false;
            GameplayEffect.TriggerOnDeactivation();
        }
        
        public bool CanApplyToTarget(AbilitySystemComponent.AbilitySystemComponent target )
        {
            return target.HasAllTags(GameplayEffect.TagContainer.ApplicationRequiredTags);
        }

        public void RefreshModifierStack()
        {
            //_modifierStack = 
        }
        
        public void Tick()
        {
            PeriodTicker?.Tick();
        }
    }
}