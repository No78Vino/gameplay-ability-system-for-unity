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
#pragma warning disable CS0067 // 事件从未使用过
        public event Action<AbilitySystemComponent, GameplayEffectSpec> onImmunity;
#pragma warning restore CS0067 // 事件从未使用过
        
        public event Action<int,int> onStackCountChanged;

        
        public GameplayEffectSpec(GameplayEffect gameplayEffect)
        {
            GameplayEffect = gameplayEffect;
            Duration = GameplayEffect.Duration;
            DurationPolicy = GameplayEffect.DurationPolicy;
            Stacking = GameplayEffect.Stacking;
            Modifiers = GameplayEffect.Modifiers;
            if (gameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant)
            {
                PeriodTicker = new GameplayEffectPeriodTicker(this);
            }
        }

        public void Init(AbilitySystemComponent source, AbilitySystemComponent owner, float level = 1)
        {
            Source = source;
            Owner = owner;
            Level = level;
            if (GameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant)
            {
                PeriodExecution = GameplayEffect.PeriodExecution?.CreateSpec(source, owner);
                SetGrantedAbility(GameplayEffect.GrantedAbilities);
            }
            CaptureAttributesSnapshot();
        }
        public GameplayEffect GameplayEffect { get; }
        public float ActivationTime { get; private set; }
        public float Level { get; private set; }
        public AbilitySystemComponent Source { get; private set; }
        public AbilitySystemComponent Owner { get; private set; }
        public bool IsApplied { get; private set; }
        public bool IsActive { get; private set; }
        public GameplayEffectPeriodTicker PeriodTicker { get; }
        public float Duration { get; private set; }
        public EffectsDurationPolicy DurationPolicy { get; private set; }
        public GameplayEffectSpec PeriodExecution { get; private set; }
        public GameplayEffectModifier[] Modifiers { get; private set; }
        public GrantedAbilitySpecFromEffect[] GrantedAbilitySpec { get; private set; }
        public GameplayEffectStacking Stacking { get; private set; }

        
        public Dictionary<string, float> SnapshotSourceAttributes { get; private set; }
        public Dictionary<string, float> SnapshotTargetAttributes { get; private set; }

        /// <summary>
        /// 堆叠数
        /// </summary>
        public int StackCount { get; private set; } = 1;
        

        public float DurationRemaining()
        {
            if (DurationPolicy == EffectsDurationPolicy.Infinite)
                return -1;

            return Mathf.Max(0, Duration - (Time.time - ActivationTime));
        }

        public void SetLevel(float level)
        {
            Level = level;
        }

        public void SetActivationTime(float activationTime)
        {
            ActivationTime = activationTime;
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

        public void SetModifiers(GameplayEffectModifier[] modifiers)
        {
            Modifiers = modifiers;
        }

        public void SetGrantedAbility(GrantedAbilityFromEffect[] grantedAbility)
        {
            GrantedAbilitySpec = new GrantedAbilitySpecFromEffect[grantedAbility.Length];
            for (var i = 0; i < grantedAbility.Length; i++)
            {
                GrantedAbilitySpec[i] = grantedAbility[i].CreateSpec(this);
            }
        }

        public void SetStacking(GameplayEffectStacking stacking)
        {
            Stacking = stacking;
        }

        public void Apply()
        {
            if (IsApplied) return;
            IsApplied = true;

            if (GameplayEffect.CanRunning(Owner))
            {
                Activate();
            }
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
            ActivationTime = Time.time;
            TriggerOnActivation();
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
            TriggerOnDeactivation();
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
            Owner.GameplayEffectContainer.RemoveGameplayEffectWithAnyTags(GameplayEffect.TagContainer
                .RemoveGameplayEffectsWithTags);
            Owner.ApplyModFromInstantGameplayEffect(this);
            
            TriggerCueOnExecute();
        }

        public void TriggerOnAdd()
        {
            TriggerCueOnAdd();
        }

        public void TriggerOnRemove()
        {
            TriggerCueOnRemove();
            
            TryRemoveGrantedAbilities();
        }

        private void TriggerOnActivation()
        {
            TriggerCueOnActivation();
            Owner.GameplayTagAggregator.ApplyGameplayEffectDynamicTag(this);
            Owner.GameplayEffectContainer.RemoveGameplayEffectWithAnyTags(GameplayEffect.TagContainer
                .RemoveGameplayEffectsWithTags);
            
            TryActivateGrantedAbilities();
        }

        private void TriggerOnDeactivation()
        {
            TriggerCueOnDeactivation();
            Owner.GameplayTagAggregator.RestoreGameplayEffectDynamicTags(this);
            
            TryDeactivateGrantedAbilities();
        }

        public void TriggerOnTick()
        {
            if (DurationPolicy == EffectsDurationPolicy.Duration ||
                DurationPolicy == EffectsDurationPolicy.Infinite)
                CueOnTick();
        }

        public void TriggerOnImmunity()
        {
            // TODO 免疫触发事件逻辑需要调整
            // onImmunity?.Invoke(Owner, this);
            // onImmunity = null;
        }

        public void RemoveSelf()
        {
            Owner.GameplayEffectContainer.RemoveGameplayEffectSpec(this);
        }

        private void CaptureAttributesSnapshot()
        {
            SnapshotSourceAttributes = Source.DataSnapshot();
            SnapshotTargetAttributes = Source == Owner ? SnapshotSourceAttributes : Owner.DataSnapshot();
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
            return _valueMapWithTag.TryGetValue(tag, out var value) ? value : (float?)null;
        }

        public float? GetMapValue(string name)
        {
            return _valueMapWithName.TryGetValue(name, out var value) ? value : (float?)null;
        }
        
        private void TryActivateGrantedAbilities()
        {
            foreach (var grantedAbilitySpec in GrantedAbilitySpec)
            {
                if (grantedAbilitySpec.ActivationPolicy == GrantedAbilityActivationPolicy.SyncWithEffect)
                {
                    Owner.TryActivateAbility(grantedAbilitySpec.AbilityName);
                }
            }
        }

        private void TryDeactivateGrantedAbilities()
        {
            foreach (var grantedAbilitySpec in GrantedAbilitySpec)
            {
                if (grantedAbilitySpec.DeactivationPolicy == GrantedAbilityDeactivationPolicy.SyncWithEffect)
                {
                    Owner.TryEndAbility(grantedAbilitySpec.AbilityName);
                }
            }
        }

        private void TryRemoveGrantedAbilities()
        {
            foreach (var grantedAbilitySpec in GrantedAbilitySpec)
            {
                if (grantedAbilitySpec.RemovePolicy == GrantedAbilityRemovePolicy.SyncWithEffect)
                {
                    Owner.TryCancelAbility(grantedAbilitySpec.AbilityName);
                    Owner.RemoveAbility(grantedAbilitySpec.AbilityName);
                }
            }
        }

        #region ABOUT STACKING
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Stack Count是否变化</returns>
        public bool RefreshStack()
        {
            var oldStackCount = StackCount;
            RefreshStack(StackCount + 1);
            OnStackCountChange(oldStackCount, StackCount);
            return oldStackCount != StackCount;
        }
        
        public void RefreshStack(int stackCount)
        {
            if (stackCount <= Stacking.limitCount)
            {
                // 更新栈数
                StackCount = Mathf.Max(1,stackCount); // 最小层数为1
                // 是否刷新Duration
                if (Stacking.durationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    RefreshDuration();
                }
                // 是否重置Period
                if (Stacking.periodResetPolicy == PeriodResetPolicy.ResetOnSuccessfulApplication)
                {
                    PeriodTicker.ResetPeriod();
                }
            }
            else
            {
                // 溢出GE生效
                foreach (var overflowEffect in Stacking.overflowEffects)
                    Owner.ApplyGameplayEffectToSelf(overflowEffect);

                if (Stacking.durationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    if (Stacking.denyOverflowApplication)
                    {
                        //当DenyOverflowApplication为True是才有效，当Overflow时是否直接删除所有层数
                        if (Stacking.clearStackOnOverflow)
                        {
                            RemoveSelf();
                        }
                    }
                    else
                    {
                        RefreshDuration();
                    }
                }
            }
        }

        public void RefreshDuration()
        {
            ActivationTime = Time.time;
        }
        
        private void OnStackCountChange(int oldStackCount, int newStackCount)
        {
            
            onStackCountChanged?.Invoke(oldStackCount, newStackCount);
        }
        
        public void RegisterOnStackCountChanged(Action<int, int> callback)
        {
            onStackCountChanged += callback;
        }

        public void UnregisterOnStackCountChanged(Action<int, int> callback)
        {
            onStackCountChanged -= callback;
        }

        #endregion
    }
}