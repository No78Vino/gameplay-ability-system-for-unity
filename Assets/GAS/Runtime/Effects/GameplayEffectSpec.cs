using System;
using System.Collections.Generic;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    /// 注意: 永远不要直接持有对GameplayEffectSpec的引用, 用EntityRef代替, 否则当它回收入池再次使用时会出现问题
    /// </summary>
    public class GameplayEffectSpec : IEntity, IPool
    {
        private Dictionary<GameplayTag, float> _valueMapWithTag;
        private Dictionary<string, float> _valueMapWithName;
        private List<GameplayCueDurationalSpec> _cueDurationalSpecs;

        public object UserData { get; set; }

        /// <summary>
        /// The execution type of onImmunity is one shot.
        /// </summary>
#pragma warning disable CS0414 // The field 'GameplayEffectSpec.onImmunity' is assigned but its value is never used
        public event Action<AbilitySystemComponent, GameplayEffectSpec> onImmunity;
#pragma warning restore CS0414

        private event Action<int, int> onStackCountChanged;

        public ulong InstanceId { get; private set; }

        public bool IsFromPool { get; set; }

        public void Awake(GameplayEffect gameplayEffect, object userData = null)
        {
            InstanceId = IdGenerator.Next;

            GameplayEffect = gameplayEffect;
            UserData = userData;
            Duration = GameplayEffect.Duration;
            DurationPolicy = GameplayEffect.DurationPolicy;
            Stacking = GameplayEffect.Stacking;
            Modifiers = GameplayEffect.Modifiers;
            if (gameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant)
            {
                var periodTicker = ObjectPool.Instance.Fetch<GameplayEffectPeriodTicker>();
                periodTicker.Awake(this);
                // EntityRef之前必须确定InstanceId的值 
                PeriodTicker = periodTicker;
            }
        }

        public void Recycle()
        {
            if (InstanceId != 0)
            {
                InstanceId = 0;

                GameplayEffect = default;
                ActivationTime = default;
                Level = default;
                Source = default;
                Owner = default;
                IsApplied = default;
                IsActive = default;

                var gameplayEffectPeriodTicker = PeriodTicker.Value;
                if (gameplayEffectPeriodTicker != null)
                {
                    gameplayEffectPeriodTicker.Release();
                    ObjectPool.Instance.Recycle(gameplayEffectPeriodTicker);
                }

                PeriodTicker = default;

                Duration = default;
                DurationPolicy = default;
                PeriodExecution.Value?.Recycle();
                PeriodExecution = default;
                Modifiers = default;

                if (GrantedAbilitiesSpecFromEffect != null)
                {
                    foreach (GrantedAbilitySpecFromEffect grantedAbilitySpecFromEffect in GrantedAbilitiesSpecFromEffect)
                    {
                        if (grantedAbilitySpecFromEffect != null)
                        {
                            grantedAbilitySpecFromEffect.Release();
                            ObjectPool.Instance.Recycle(grantedAbilitySpecFromEffect);
                        }
                    }

                    GrantedAbilitiesSpecFromEffect.Clear();
                    ObjectPool.Instance.Recycle(GrantedAbilitiesSpecFromEffect);
                    GrantedAbilitiesSpecFromEffect = default;
                }

                Stacking = default;

                // 注意: SnapshotSourceAttributes 和 SnapshotTargetAttributes 可能是同一个对象
                if (SnapshotSourceAttributes != null)
                {
                    SnapshotSourceAttributes.Clear();
                    ObjectPool.Instance.Recycle(SnapshotSourceAttributes);
                }

                if (SnapshotTargetAttributes != null && SnapshotSourceAttributes != SnapshotTargetAttributes)
                {
                    SnapshotTargetAttributes.Clear();
                    ObjectPool.Instance.Recycle(SnapshotTargetAttributes);
                }

                SnapshotSourceAttributes = null;
                SnapshotTargetAttributes = null;

                StackCount = 1;

                if (_valueMapWithTag != null)
                {
                    _valueMapWithTag.Clear();
                    ObjectPool.Instance.Recycle(_valueMapWithTag);
                    _valueMapWithTag = null;
                }

                if (_valueMapWithName != null)
                {
                    _valueMapWithName.Clear();
                    ObjectPool.Instance.Recycle(_valueMapWithName);
                    _valueMapWithName = null;
                }

                ReleaseCueDurationalSpecs();

                onImmunity = default;
                onStackCountChanged = default;
            }

            ObjectPool.Instance.Recycle(this);
        }

        public void Init(AbilitySystemComponent source, AbilitySystemComponent owner, float level = 1)
        {
            Source = source;
            Owner = owner;
            Level = level;
            if (GameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant)
            {
                if (GameplayEffect.PeriodExecution is not null)
                {
                    PeriodExecution = GameplayEffect.PeriodExecution.CreateSpec(source, owner);
                }

                SetGrantedAbility(GameplayEffect.GrantedAbilities);
            }

            CaptureAttributesSnapshot();
        }

        public GameplayEffect GameplayEffect { get; private set; }
        public float ActivationTime { get; private set; }
        public float Level { get; private set; }
        public AbilitySystemComponent Source { get; private set; }
        public AbilitySystemComponent Owner { get; private set; }
        public bool IsApplied { get; private set; }
        public bool IsActive { get; private set; }
        internal EntityRef<GameplayEffectPeriodTicker> PeriodTicker { get; private set; }
        public float Duration { get; private set; }
        public EffectsDurationPolicy DurationPolicy { get; private set; }
        public EntityRef<GameplayEffectSpec> PeriodExecution { get; private set; }
        public GameplayEffectModifier[] Modifiers { get; private set; }
        public List<EntityRef<GrantedAbilitySpecFromEffect>> GrantedAbilitiesSpecFromEffect { get; private set; }
        public GameplayEffectStacking Stacking { get; private set; }

        public GameplayEffectSnapshotPolicy SnapshotPolicy => GameplayEffect.SnapshotPolicy;
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
            PeriodExecution.Value?.Recycle();
            PeriodExecution = periodExecution;
        }

        public void SetModifiers(GameplayEffectModifier[] modifiers)
        {
            Modifiers = modifiers;
        }

        public void SetGrantedAbility(GrantedAbilityFromEffect[] grantedAbilityFromEffects)
        {
            ReleaseGrantedAbilitiesSpecFromEffect();

            if (grantedAbilityFromEffects is null) return;
            if (grantedAbilityFromEffects.Length == 0) return;

            GrantedAbilitiesSpecFromEffect = ObjectPool.Instance.Fetch<List<EntityRef<GrantedAbilitySpecFromEffect>>>();
            foreach (var grantedAbilityFromEffect in grantedAbilityFromEffects)
            {
                GrantedAbilitiesSpecFromEffect.Add(grantedAbilityFromEffect.CreateSpec(this));
            }
        }

        private void ReleaseGrantedAbilitiesSpecFromEffect()
        {
            if (GrantedAbilitiesSpecFromEffect == null) return;
            foreach (var grantedAbilitySpecFromEffectRef in GrantedAbilitiesSpecFromEffect)
            {
                var grantedAbilitySpecFromEffect = grantedAbilitySpecFromEffectRef.Value;
                if (grantedAbilitySpecFromEffect != null)
                {
                    grantedAbilitySpecFromEffect.Release();
                    ObjectPool.Instance.Recycle(grantedAbilitySpecFromEffect);
                }
            }

            GrantedAbilitiesSpecFromEffect.Clear();
            ObjectPool.Instance.Recycle(GrantedAbilitiesSpecFromEffect);
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
            PeriodTicker.Value?.Tick();
        }

        void TriggerInstantCues(GameplayCueInstant[] cues)
        {
            try
            {
                foreach (var cue in cues) cue.ApplyFrom(this);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void TriggerCueOnExecute()
        {
            if (GameplayEffect.CueOnExecute == null || GameplayEffect.CueOnExecute.Length <= 0) return;
            TriggerInstantCues(GameplayEffect.CueOnExecute);
        }

        private void TriggerCueOnAdd()
        {
            if (GameplayEffect.CueOnAdd is { Length: > 0 })
                TriggerInstantCues(GameplayEffect.CueOnAdd);

            try
            {
                ReleaseCueDurationalSpecs();
                if (GameplayEffect.CueDurational is { Length: > 0 })
                {
                    _cueDurationalSpecs = ObjectPool.Instance.Fetch<List<GameplayCueDurationalSpec>>();
                    foreach (var cueDurational in GameplayEffect.CueDurational)
                    {
                        var cueSpec = cueDurational.ApplyFrom(this);
                        if (cueSpec != null) _cueDurationalSpecs.Add(cueSpec);
                    }

                    foreach (var cue in _cueDurationalSpecs)
                        cue.OnAdd();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void TriggerCueOnRemove()
        {
            if (GameplayEffect.CueOnRemove is { Length: > 0 })
                TriggerInstantCues(GameplayEffect.CueOnRemove);

            try
            {
                if (_cueDurationalSpecs != null)
                {
                    foreach (var cue in _cueDurationalSpecs)
                        cue.OnRemove();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                ReleaseCueDurationalSpecs();
            }
        }


        private void TriggerCueOnActivation()
        {
            if (GameplayEffect.CueOnActivate is { Length: > 0 })
                TriggerInstantCues(GameplayEffect.CueOnActivate);

            try
            {
                if (_cueDurationalSpecs != null)
                {
                    foreach (var cue in _cueDurationalSpecs)
                        cue.OnGameplayEffectActivate();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void TriggerCueOnDeactivation()
        {
            if (GameplayEffect.CueOnDeactivate is { Length: > 0 })
                TriggerInstantCues(GameplayEffect.CueOnDeactivate);

            try
            {
                if (_cueDurationalSpecs != null)
                {
                    foreach (var cue in _cueDurationalSpecs)
                        cue.OnGameplayEffectDeactivate();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void CueOnTick()
        {
            try
            {
                if (_cueDurationalSpecs != null)
                {
                    foreach (var cue in _cueDurationalSpecs)
                        cue.OnTick();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
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
            switch (SnapshotPolicy)
            {
                case GameplayEffectSnapshotPolicy.Specified:
                    if (GameplayEffect.SpecifiedSnapshotConfigs != null)
                    {
                        foreach (var config in GameplayEffect.SpecifiedSnapshotConfigs)
                        {
                            switch (config.SnapshotTarget)
                            {
                                case GameplayEffectSpecifiedSnapshotConfig.ESnapshotTarget.Source:
                                {
                                    SnapshotSourceAttributes ??= ObjectPool.Instance.Fetch<Dictionary<string, float>>();
                                    var attribute = Source.AttributeSetContainer.GetAttributeAttributeValue(config.AttributeSetName, config.AttributeShortName);
                                    if (attribute != null)
                                    {
                                        SnapshotSourceAttributes[config.AttributeName] = attribute.Value.CurrentValue;
                                    }
                                    else
                                    {
                                        Debug.LogError($"Snapshot Source Attribute \"{config.AttributeName}\" not found in AttributeSet \"{config.AttributeSetName}\"");
                                    }

                                    break;
                                }
                                case GameplayEffectSpecifiedSnapshotConfig.ESnapshotTarget.Target:
                                {
                                    SnapshotTargetAttributes ??= ObjectPool.Instance.Fetch<Dictionary<string, float>>();
                                    var attribute = Owner.AttributeSetContainer.GetAttributeAttributeValue(config.AttributeSetName, config.AttributeShortName);
                                    if (attribute != null)
                                    {
                                        SnapshotTargetAttributes[config.AttributeName] = attribute.Value.CurrentValue;
                                    }
                                    else
                                    {
                                        Debug.LogError($"Snapshot Target Attribute {config.AttributeName} not found in AttributeSet {config.AttributeSetName}");
                                    }

                                    break;
                                }
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }

                    break;
                case GameplayEffectSnapshotPolicy.AllOfSource:
                    SnapshotSourceAttributes = Source.DataSnapshot();
                    break;
                case GameplayEffectSnapshotPolicy.AllOfTarget:
                    SnapshotTargetAttributes = Owner.DataSnapshot();
                    break;
                case GameplayEffectSnapshotPolicy.AllOfBoth:
                    SnapshotSourceAttributes = Source.DataSnapshot();
                    SnapshotTargetAttributes = Source == Owner && SnapshotSourceAttributes != null ? SnapshotSourceAttributes : Owner.DataSnapshot();
                    break;
                default:
                    Debug.LogError($"Unsupported SnapshotPolicy: {SnapshotPolicy}, GameplayEffect: {GameplayEffect.GameplayEffectName}");
                    break;
            }
        }

        public void RegisterValue(GameplayTag tag, float value)
        {
            _valueMapWithTag ??= ObjectPool.Instance.Fetch<Dictionary<GameplayTag, float>>();
            _valueMapWithTag[tag] = value;
        }

        public void RegisterValue(string name, float value)
        {
            _valueMapWithName ??= ObjectPool.Instance.Fetch<Dictionary<string, float>>();
            _valueMapWithName[name] = value;
        }

        public bool UnregisterValue(GameplayTag tag)
        {
            if (_valueMapWithTag == null) return false;
            return _valueMapWithTag.Remove(tag);
        }

        public bool UnregisterValue(string name)
        {
            if (_valueMapWithName == null) return false;
            return _valueMapWithName.Remove(name);
        }

        public float? GetMapValue(GameplayTag tag)
        {
            if (_valueMapWithTag == null) return null;
            return _valueMapWithTag.TryGetValue(tag, out var value) ? value : (float?)null;
        }

        public float? GetMapValue(string name)
        {
            if (_valueMapWithName == null) return null;
            return _valueMapWithName.TryGetValue(name, out var value) ? value : (float?)null;
        }

        private void TryActivateGrantedAbilities()
        {
            if (GrantedAbilitiesSpecFromEffect == null) return;
            foreach (GrantedAbilitySpecFromEffect grantedAbilitySpec in GrantedAbilitiesSpecFromEffect)
            {
                if (grantedAbilitySpec is { ActivationPolicy: GrantedAbilityActivationPolicy.SyncWithEffect })
                {
                    Owner.TryActivateAbility(grantedAbilitySpec.AbilityName);
                }
            }
        }

        private void TryDeactivateGrantedAbilities()
        {
            if (GrantedAbilitiesSpecFromEffect == null) return;
            foreach (GrantedAbilitySpecFromEffect grantedAbilitySpec in GrantedAbilitiesSpecFromEffect)
            {
                if (grantedAbilitySpec is { DeactivationPolicy: GrantedAbilityDeactivationPolicy.SyncWithEffect })
                {
                    Owner.TryEndAbility(grantedAbilitySpec.AbilityName);
                }
            }
        }

        private void TryRemoveGrantedAbilities()
        {
            if (GrantedAbilitiesSpecFromEffect == null) return;
            foreach (GrantedAbilitySpecFromEffect grantedAbilitySpec in GrantedAbilitiesSpecFromEffect)
            {
                if (grantedAbilitySpec is { RemovePolicy: GrantedAbilityRemovePolicy.SyncWithEffect })
                {
                    Owner.TryCancelAbility(grantedAbilitySpec.AbilityName);
                    Owner.RemoveAbility(grantedAbilitySpec.AbilityName);
                }
            }
        }

        private void ReleaseCueDurationalSpecs()
        {
            if (_cueDurationalSpecs != null)
            {
                _cueDurationalSpecs.Clear();
                ObjectPool.Instance.Recycle(_cueDurationalSpecs);
                _cueDurationalSpecs = null;
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
                StackCount = Mathf.Max(1, stackCount); // 最小层数为1
                // 是否刷新Duration
                if (Stacking.durationRefreshPolicy == DurationRefreshPolicy.RefreshOnSuccessfulApplication)
                {
                    RefreshDuration();
                }

                // 是否重置Period
                if (Stacking.periodResetPolicy == PeriodResetPolicy.ResetOnSuccessfulApplication)
                {
                    PeriodTicker.Value.ResetPeriod();
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