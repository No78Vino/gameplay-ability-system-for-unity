using System.Collections.Generic;
using GAS.General;
using GAS.Runtime.Component;
using GAS.Runtime.Cue;
using Unity.Mathematics;

namespace GAS.Runtime.Effects
{
    public class GameplayEffectSpec
    {
        private GameplayCueDurationalSpec[] _cueDurationalSpecs;

        /// <summary>
        ///     If the gameplay effect has a period and the execution is not null,
        ///     this is the execution that will be triggered every period.
        /// </summary>
        public GameplayEffectSpec PeriodExecution;

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
            Duration = GameplayEffect.Duration;
            if (gameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant)
                PeriodTicker = new GameplayEffectPeriodTicker(this);

            CaptureDataFromSource();
        }

        public GameplayEffect GameplayEffect { get; }
        public long ActivationTime { get; private set; }
        public float Level { get; private set; }
        public AbilitySystemComponent Source { get; }
        public AbilitySystemComponent Owner { get; }
        public bool IsApplied { get; private set; }
        public bool IsActive { get; private set; }
        public GameplayEffectPeriodTicker PeriodTicker { get; }
        public float Duration { get; private set; } 

        public Dictionary<string, float> SnapshotAttributes { get; private set; }

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

        public void SetDuration(float duration)
        {
            Duration = duration;
        }
        
        public void Apply()
        {
            if (IsApplied) return;
            IsApplied = true;
            Activate();
        }

        public void DisApply()
        {
            if (!IsApplied) return;
            IsApplied = false;
            Deactivate();
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

        private void TriggerCueOnExecute()
        {
            if (GameplayEffect.CueOnExecute == null || GameplayEffect.CueOnExecute.Length <= 0) return;
            foreach (var cue in GameplayEffect.CueOnExecute)
            {
                var instantCue = cue.CreateSpec(new GameplayCueParameters() { sourceGameplayEffectSpec = this });
                instantCue?.Trigger();
            }
        }

        private void TriggerCueOnAdd()
        {
            if (GameplayEffect.CueOnAdd!=null&&GameplayEffect.CueOnAdd.Length > 0)
                foreach (var cue in GameplayEffect.CueOnAdd)
                {
                    var instantCue = cue.CreateSpec(new GameplayCueParameters(){sourceGameplayEffectSpec = this} );
                    instantCue?.Trigger();
                }

            if (GameplayEffect.CueDurational!=null && GameplayEffect.CueDurational.Length > 0)
            {
                _cueDurationalSpecs = new GameplayCueDurationalSpec[GameplayEffect.CueDurational.Length];
                for (var i = 0; i < GameplayEffect.CueDurational.Length; i++)
                {
                    var cueDurational = GameplayEffect.CueDurational[i];
                    _cueDurationalSpecs[i] = cueDurational.CreateSpec(new GameplayCueParameters(){sourceGameplayEffectSpec = this} );
                }

                foreach (var cue in _cueDurationalSpecs) cue.OnAdd();
            }
        }

        private void TriggerCueOnRemove()
        {
            if (GameplayEffect.CueOnRemove!=null && GameplayEffect.CueOnRemove.Length > 0)
                foreach (var cue in GameplayEffect.CueOnRemove)
                {
                    var instantCue = cue.CreateSpec(new GameplayCueParameters(){sourceGameplayEffectSpec = this} );
                    instantCue?.Trigger();
                }

            if (GameplayEffect.CueDurational != null && GameplayEffect.CueDurational.Length > 0)
            {
                foreach (var cue in _cueDurationalSpecs) cue.OnRemove();

                _cueDurationalSpecs = null;
            }
        }

        private void TriggerCueOnActivation()
        {
            if (GameplayEffect.CueOnActivate!=null && GameplayEffect.CueOnActivate.Length > 0)
                foreach (var cue in GameplayEffect.CueOnActivate)
                {
                    var instantCue = cue.CreateSpec(new GameplayCueParameters(){sourceGameplayEffectSpec = this} );
                    instantCue?.Trigger();
                }

            if (GameplayEffect.CueDurational!=null && GameplayEffect.CueDurational.Length > 0)
                foreach (var cue in _cueDurationalSpecs)
                    cue.OnGameplayEffectActivate();
        }

        private void TriggerCueOnDeactivation()
        {
            if (GameplayEffect.CueOnDeactivate!=null && GameplayEffect.CueOnDeactivate.Length > 0)
                foreach (var cue in GameplayEffect.CueOnDeactivate)
                {
                    var instantCue = cue.CreateSpec(new GameplayCueParameters(){sourceGameplayEffectSpec = this} );
                    instantCue?.Trigger();
                }

            if (GameplayEffect.CueDurational!=null && GameplayEffect.CueDurational.Length > 0)
                foreach (var cue in _cueDurationalSpecs)
                    cue.OnGameplayEffectDeactivate();
        }

        private void CueOnTick()
        {
            if (GameplayEffect.CueDurational==null || GameplayEffect.CueDurational.Length <= 0) return;
            foreach (var cue in _cueDurationalSpecs) cue.OnTick();
        }

        public void TriggerOnExecute()
        {
            TriggerCueOnExecute();

            Owner.GameplayEffectContainer.RemoveGameplayEffectWithAnyTags(GameplayEffect.TagContainer
                .RemoveGameplayEffectsWithTags);
            Owner.ApplyModFromInstantGameplayEffect(this);
        }

        public void TriggerOnAdd()
        {
            TriggerCueOnAdd();
        }

        public void TriggerOnRemove()
        {
            TriggerCueOnRemove();
        }

        private void TriggerOnActivation()
        {
            TriggerCueOnActivation();
            Owner.GameplayTagAggregator.ApplyGameplayEffectDynamicTag(this);
            Owner.GameplayEffectContainer.RemoveGameplayEffectWithAnyTags(GameplayEffect.TagContainer
                .RemoveGameplayEffectsWithTags);
        }

        private void TriggerOnDeactivation()
        {
            TriggerCueOnDeactivation();
            Owner.GameplayTagAggregator.RestoreGameplayEffectDynamicTags(this);
        }

        public void TriggerOnTick()
        {
            if (GameplayEffect.DurationPolicy == EffectsDurationPolicy.Duration ||
                GameplayEffect.DurationPolicy == EffectsDurationPolicy.Infinite)
                CueOnTick();
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