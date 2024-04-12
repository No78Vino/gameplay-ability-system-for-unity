using System;
using System.Collections.Generic;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    public class GameplayEffectSpec
    {
        private Dictionary<GameplayTag, float> _valueMapWithTag = new Dictionary<GameplayTag, float>();
        private Dictionary<string, float> _valueMapWithName = new Dictionary<string, float>();
        private List<GameplayCueDurationalSpec> _cueDurationalSpecs = new List<GameplayCueDurationalSpec>();
        
        /// <summary>
        /// The execution type of onImmunity is one shot.
        /// </summary>
        public event Action<AbilitySystemComponent,GameplayEffectSpec> onImmunity; 

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
            DurationPolicy = GameplayEffect.DurationPolicy;
            if (gameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant)
            {
                PeriodExecution = GameplayEffect.PeriodExecution?.CreateSpec(source, owner);
                PeriodTicker = new GameplayEffectPeriodTicker(this);
            }

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
        public EffectsDurationPolicy DurationPolicy { get; private set; }
        public GameplayEffectSpec PeriodExecution{ get; private set; }

        public Dictionary<string, float> SnapshotAttributes { get; private set; }

        public float DurationRemaining()
        {
            if (DurationPolicy == EffectsDurationPolicy.Infinite)
                return -1;

            return Mathf.Max(0, Duration - (GASTimer.Timestamp() - ActivationTime) / 1000f);
        }

        public void SetLevel(float level)
        {
            Level = level;
        }

        public void SetDuration(float duration)
        {
            Duration = duration;
        }
        
        public void SetDurationPolicy(EffectsDurationPolicy durationPolicy)
        {
            DurationPolicy = durationPolicy;
        }
        
        public void SetPeriodExecution(GameplayEffectSpec periodExecution)
        {
            PeriodExecution = periodExecution;
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
            return Owner.HasAllTags(GameplayEffect.TagContainer.OngoingRequiredTags);
        }

        public void Tick()
        {
            PeriodTicker?.Tick();
        }

        void TriggerInstantCues(GameplayCueInstant[] cues)
        {
            foreach (var cue in cues) cue.ApplyFrom(this);
        }
        
        private void TriggerCueOnExecute()
        {
            if (GameplayEffect.CueOnExecute == null || GameplayEffect.CueOnExecute.Length <= 0) return;
            TriggerInstantCues(GameplayEffect.CueOnExecute);
        }

        private void TriggerCueOnAdd()
        {
            if (GameplayEffect.CueOnAdd != null && GameplayEffect.CueOnAdd.Length > 0)
                TriggerInstantCues(GameplayEffect.CueOnAdd);

            if (GameplayEffect.CueDurational != null && GameplayEffect.CueDurational.Length > 0)
            {
                _cueDurationalSpecs.Clear();
                foreach (var cueDurational in GameplayEffect.CueDurational)
                {
                    var cueSpec = cueDurational.ApplyFrom(this);
                    if (cueSpec != null) _cueDurationalSpecs.Add(cueSpec);
                }

                foreach (var cue in _cueDurationalSpecs) cue.OnAdd();
            }
        }

        private void TriggerCueOnRemove()
        {
            if (GameplayEffect.CueOnRemove != null && GameplayEffect.CueOnRemove.Length > 0)
                TriggerInstantCues(GameplayEffect.CueOnRemove);

            if (GameplayEffect.CueDurational != null && GameplayEffect.CueDurational.Length > 0)
            {
                foreach (var cue in _cueDurationalSpecs) cue.OnRemove();

                _cueDurationalSpecs = null;
            }
        }

        private void TriggerCueOnActivation()
        {
            if (GameplayEffect.CueOnActivate != null && GameplayEffect.CueOnActivate.Length > 0)
                TriggerInstantCues(GameplayEffect.CueOnActivate);

            if (GameplayEffect.CueDurational != null && GameplayEffect.CueDurational.Length > 0)
                foreach (var cue in _cueDurationalSpecs)
                    cue.OnGameplayEffectActivate();
        }

        private void TriggerCueOnDeactivation()
        {
            if (GameplayEffect.CueOnDeactivate != null && GameplayEffect.CueOnDeactivate.Length > 0)
                TriggerInstantCues(GameplayEffect.CueOnDeactivate);

            if (GameplayEffect.CueDurational != null && GameplayEffect.CueDurational.Length > 0)
                foreach (var cue in _cueDurationalSpecs)
                    cue.OnGameplayEffectDeactivate();
        }

        private void CueOnTick()
        {
            if (GameplayEffect.CueDurational == null || GameplayEffect.CueDurational.Length <= 0) return;
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
            if (DurationPolicy == EffectsDurationPolicy.Duration ||
                DurationPolicy == EffectsDurationPolicy.Infinite)
                CueOnTick();
        }

        public void TriggerOnImmunity()
        {
            onImmunity?.Invoke(Owner, this);
            onImmunity = null;
        }
        
        public void RemoveSelf()
        {
            Owner.GameplayEffectContainer.RemoveGameplayEffectSpec(this);
        }

        private void CaptureDataFromSource()
        {
            SnapshotAttributes = Source.DataSnapshot();
        }

        public void RegisterValue(GameplayTag tag, float value)
        {
            _valueMapWithTag[tag] = value;
        }
        
        public void RegisterValue(string name, float value)
        {
            _valueMapWithName[name] = value;
        }
        
        public bool UnregisterValue(GameplayTag tag)
        {
            return _valueMapWithTag.Remove(tag);
        }
        
        public bool UnregisterValue(string name)
        {
            return _valueMapWithName.Remove(name);
        }
        
        public float? GetMapValue(GameplayTag tag)
        {
            return _valueMapWithTag.TryGetValue(tag, out var value) ? value : (float?) null;
        }
        
        public float? GetMapValue(string name)
        {
            return _valueMapWithName.TryGetValue(name, out var value) ? value : (float?) null;
        }
    }
}