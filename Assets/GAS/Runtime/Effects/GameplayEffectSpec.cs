using System.Collections.Generic;
using GAS.General;
using GAS.Runtime.Attribute;
using GAS.Runtime.Component;
using Unity.Mathematics;

namespace GAS.Runtime.Effects
{
    public class GameplayEffectSpec
    {
        public delegate void GameplayEffectEventHandler(GameplayEffectSpec sender);

        public GameplayEffectSpec(
            GameplayEffect gameplayEffect,
            AbilitySystemComponent source,
            AbilitySystemComponent owner,
            float level = 1)
        {
            GameplayEffect = gameplayEffect;
            Source = source;
            Owner = owner;
            Level = level;

            if (gameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant)
                PeriodTicker = new GameplayEffectPeriodTicker(this);

            CaptureDataFromSource();
        }

        public GameplayEffect GameplayEffect { get; }
        public long ActivationTime { get; private set; }
        public float Level { get; private set; }
        public AbilitySystemComponent Source { get; private set; }
        public AbilitySystemComponent Owner { get; }
        public bool IsActive { get; private set; }
        public GameplayEffectPeriodTicker PeriodTicker { get; }
        public float Duration => GameplayEffect.Duration;

        /// <summary>
        /// If the gameplay effect has a period and the execution is not null,
        /// this is the execution that will be triggered every period.
        /// </summary>
        public GameplayEffectSpec PeriodExecution;
        
        public Dictionary<string,float> SnapshotAttributes { get;private set; }

        public float DurationRemaining()
        {
            if (GameplayEffect.DurationPolicy == EffectsDurationPolicy.Infinite)
                return -1;

            return math.max(0, Duration - (GASTimer.Timestamp() - ActivationTime) / 1000f);
        }

        public void SetLevel(float level)
        {
            Level = level;
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
            ActivationTime = GASTimer.Timestamp();
            TriggerOnActivation();
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
            TriggerOnDeactivation();
        }

        public bool CanRunning()
        {
            var canRunning = Owner.HasAllTags(GameplayEffect.TagContainer.OngoingRequiredTags);
            return canRunning;
        }

        public void Tick()
        {
            PeriodTicker?.Tick();
        }

        public event GameplayEffectEventHandler OnExecute;
        public event GameplayEffectEventHandler OnAdd;
        public event GameplayEffectEventHandler OnRemove;
        public event GameplayEffectEventHandler OnActivation;
        public event GameplayEffectEventHandler OnDeactivation;
        public event GameplayEffectEventHandler OnTick;

        public void TriggerOnExecute()
        {
            OnExecute?.Invoke(this);
            GameplayEffect.TriggerCueOnExecute();
            
            Owner.ApplyModFromInstantGameplayEffect(this);
        }
        
        public void TriggerOnAdd()
        {
            OnAdd?.Invoke(this);
            GameplayEffect.TriggerCueOnAdd();
        }

        public void TriggerOnRemove()
        {
            OnRemove?.Invoke(this);
            GameplayEffect.TriggerCueOnRemove();
        }

        public void TriggerOnActivation()
        {            
            OnActivation?.Invoke(this);
            Owner.AddTags(GameplayEffect.TagContainer.GrantedTags);
        }

        public void TriggerOnDeactivation()
        {
            OnDeactivation?.Invoke(this);
            Owner.RemoveTags(GameplayEffect.TagContainer.GrantedTags);
        }

        public void TriggerOnTick()
        {
            if (GameplayEffect.DurationPolicy == EffectsDurationPolicy.Duration||
                GameplayEffect.DurationPolicy == EffectsDurationPolicy.Infinite)
            {
                OnTick?.Invoke(this);
            }
        }

        public void RemoveSelf()
        {
            Owner.GameplayEffectContainer.RemoveGameplayEffectSpec(this);
        }

        private void CaptureDataFromSource()
        {
            SnapshotAttributes = Source.DataSnapshot();
        }
    }
}