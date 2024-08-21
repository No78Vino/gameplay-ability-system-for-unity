using System;
using System.Collections.Generic;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    public class GameplayEffectContainer
    {
        private readonly AbilitySystemComponent _owner;
        private readonly List<GameplayEffectSpec> _gameplayEffectSpecs = new();

        public GameplayEffectContainer(AbilitySystemComponent owner)
        {
            _owner = owner;
        }

        private event Action OnGameplayEffectContainerIsDirty;

        public List<GameplayEffectSpec> GameplayEffects()
        {
            return _gameplayEffectSpecs;
        }

        public void Tick()
        {
            var gameplayEffectSpecs = ObjectPool.Instance.Fetch<List<GameplayEffectSpec>>();
            gameplayEffectSpecs.AddRange(_gameplayEffectSpecs);

            foreach (var gameplayEffectSpec in gameplayEffectSpecs)
            {
                if (gameplayEffectSpec.IsActive)
                {
                    gameplayEffectSpec.Tick();
                }
            }

            gameplayEffectSpecs.Clear();
            ObjectPool.Instance.Recycle(gameplayEffectSpecs);
        }

        public void RegisterOnGameplayEffectContainerIsDirty(Action action)
        {
            OnGameplayEffectContainerIsDirty += action;
        }

        public void UnregisterOnGameplayEffectContainerIsDirty(Action action)
        {
            OnGameplayEffectContainerIsDirty -= action;
        }

        public void RemoveGameplayEffectWithAnyTags(in GameplayTagSet tags)
        {
            if (tags.Empty) return;

            var removeList = ObjectPool.Instance.Fetch<List<GameplayEffectSpec>>();

            foreach (var gameplayEffectSpec in _gameplayEffectSpecs)
            {
                var assetTags = gameplayEffectSpec.GameplayEffect.TagContainer.AssetTags;
                if (!assetTags.Empty && assetTags.HasAnyTags(tags))
                {
                    removeList.Add(gameplayEffectSpec);
                    continue;
                }

                var grantedTags = gameplayEffectSpec.GameplayEffect.TagContainer.GrantedTags;
                if (!grantedTags.Empty && grantedTags.HasAnyTags(tags)) removeList.Add(gameplayEffectSpec);
            }

            foreach (var gameplayEffectSpec in removeList) RemoveGameplayEffectSpec(gameplayEffectSpec);

            removeList.Clear();
            ObjectPool.Instance.Recycle(removeList);
        }

        /// <summary>
        /// </summary>
        /// <returns>
        ///    返回实际的GameplayEffectSpec(不一定是传入的Spec, 如果堆叠成功返回的是被堆叠的初始Spec),
        ///    如果返回null, 要么是一次性生效的, 要么是应用失败的(比如被免疫)
        /// </returns>
        public EntityRef<GameplayEffectSpec> AddGameplayEffectSpec(AbilitySystemComponent source, EntityRef<GameplayEffectSpec> effectSpecRef, bool overwriteEffectLevel = false, int effectLevel = 0)
        {
            var effectSpec = effectSpecRef.Value;
            if (effectSpec == null)
                return null;

            if (!effectSpec.GameplayEffect.CanApplyTo(_owner))
            {
                effectSpec.Recycle();
                return null;
            }

            if (effectSpec.GameplayEffect.IsImmune(_owner))
            {
                var level = overwriteEffectLevel ? effectLevel : source.Level;
                effectSpec.Init(source, _owner, level);
                effectSpec.TriggerOnImmunity();
                effectSpec.Recycle();
                return null;
            }

            if (effectSpec.DurationPolicy == EffectsDurationPolicy.Instant)
            {
                var level = overwriteEffectLevel ? effectLevel : source.Level;
                effectSpec.Init(source, _owner, level);
                effectSpec.TriggerOnExecute();
                effectSpec.Recycle();
                return null;
            }

            // Check GE Stacking
            // 处理GE堆叠
            switch (effectSpec.Stacking.stackingType)
            {
                case StackingType.None:
                {
                    Operation_AddNewGameplayEffectSpec(source, ref effectSpecRef, overwriteEffectLevel, effectLevel);
                    return effectSpecRef;
                }
                case StackingType.AggregateByTarget:
                {
                    GetStackingEffectSpecByData(effectSpec.GameplayEffect, out var geSpec);
                    if (geSpec == null)
                    {
                        Operation_AddNewGameplayEffectSpec(source, ref effectSpecRef, overwriteEffectLevel, effectLevel);
                        return effectSpecRef;
                    }

                    bool stackCountChange = geSpec.RefreshStack();
                    if (stackCountChange) OnRefreshStackCountMakeContainerDirty();

                    effectSpec.Recycle();
                    return geSpec.IsApplied ? geSpec : null;
                }
                case StackingType.AggregateBySource:
                {
                    GetStackingEffectSpecByDataFrom(effectSpec.GameplayEffect, source, out var geSpec);
                    if (geSpec == null)
                    {
                        Operation_AddNewGameplayEffectSpec(source, ref effectSpecRef, overwriteEffectLevel, effectLevel);
                        return effectSpecRef;
                    }

                    bool stackCountChange = geSpec.RefreshStack();
                    if (stackCountChange) OnRefreshStackCountMakeContainerDirty();

                    effectSpec.Recycle();
                    return geSpec.IsApplied ? geSpec : null;
                }
                default:
                {
                    Debug.LogError("Unsupported StackingType: " + effectSpec.Stacking.stackingType);
                    effectSpec.Recycle();
                    return null;
                }
            }
        }

        public void RemoveGameplayEffectSpec(GameplayEffectSpec spec)
        {
            if (spec == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("the GameplayEffectSpec you want to remove is null!");
#endif
                return;
            }

            spec.DisApply();
            spec.TriggerOnRemove();
            _gameplayEffectSpecs.Remove(spec);
            spec.Recycle();

            OnGameplayEffectContainerIsDirty?.Invoke();
        }

        public void RemoveGameplayEffectSpec(in EntityRef<GameplayEffectSpec> gameplayEffectSpecRef)
        {
            var gameplayEffectSpec = gameplayEffectSpecRef.Value;
            if (gameplayEffectSpec == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("the EntityRef of GameplayEffectSpec is Invalid!");
#endif
                return;
            }

            RemoveGameplayEffectSpec(gameplayEffectSpec);
        }

        public void RefreshGameplayEffectState()
        {
            bool isDirty = false;
            foreach (var gameplayEffectSpec in _gameplayEffectSpecs)
            {
                if (gameplayEffectSpec.IsApplied)
                {
                    if (gameplayEffectSpec.IsActive)
                    {
                        if (!gameplayEffectSpec.GameplayEffect.CanRunning(_owner))
                        {
                            isDirty = true;
                            gameplayEffectSpec.Deactivate();
                        }
                    }
                    else
                    {
                        if (gameplayEffectSpec.GameplayEffect.CanRunning(_owner))
                        {
                            isDirty = true;
                            gameplayEffectSpec.Activate();
                        }
                    }
                }
            }

            if (isDirty)
            {
                OnGameplayEffectContainerIsDirty?.Invoke();
            }
        }

        public CooldownTimer CheckCooldownFromTags(in GameplayTagSet tags)
        {
            float longestCooldown = 0;
            float maxDuration = 0;

            // Check if the cooldown tag is granted to the player, and if so, capture the remaining duration for that tag
            foreach (var spec in _gameplayEffectSpecs)
            {
                if (spec.IsActive)
                {
                    var grantedTags = spec.GameplayEffect.TagContainer.GrantedTags;
                    if (grantedTags.Empty) continue;
                    foreach (var t in grantedTags.Tags)
                    foreach (var targetTag in tags.Tags)
                    {
                        if (t != targetTag) continue;
                        // If this is an infinite GE, then return null to signify this is on CD
                        if (spec.GameplayEffect.DurationPolicy == EffectsDurationPolicy.Infinite)
                            return new CooldownTimer { TimeRemaining = -1, Duration = 0 };

                        var durationRemaining = spec.DurationRemaining();

                        if (!(durationRemaining > longestCooldown)) continue;
                        longestCooldown = durationRemaining;
                        maxDuration = spec.Duration;
                    }
                }
            }

            return new CooldownTimer { TimeRemaining = longestCooldown, Duration = maxDuration };
        }

        public void ClearGameplayEffect()
        {
            bool isDirty = _gameplayEffectSpecs.Count > 0;

            foreach (var gameplayEffectSpec in _gameplayEffectSpecs)
            {
                gameplayEffectSpec.DisApply();
                gameplayEffectSpec.TriggerOnRemove();
                gameplayEffectSpec.Recycle();
            }

            _gameplayEffectSpecs.Clear();

            if (isDirty)
            {
                OnGameplayEffectContainerIsDirty?.Invoke();
            }
        }

        private void GetStackingEffectSpecByData(GameplayEffect effect, out GameplayEffectSpec spec)
        {
            foreach (var gameplayEffectSpec in _gameplayEffectSpecs)
                if (gameplayEffectSpec.GameplayEffect.StackEqual(effect))
                {
                    spec = gameplayEffectSpec;
                    return;
                }

            spec = null;
        }

        private void GetStackingEffectSpecByDataFrom(GameplayEffect effect, AbilitySystemComponent source,
            out GameplayEffectSpec spec)
        {
            foreach (var gameplayEffectSpec in _gameplayEffectSpecs)
                if (gameplayEffectSpec.Source == source &&
                    gameplayEffectSpec.GameplayEffect.StackEqual(effect))
                {
                    spec = gameplayEffectSpec;
                    return;
                }

            spec = null;
        }

        private void OnRefreshStackCountMakeContainerDirty()
        {
            OnGameplayEffectContainerIsDirty?.Invoke();
        }

        private void Operation_AddNewGameplayEffectSpec(AbilitySystemComponent source, ref EntityRef<GameplayEffectSpec> effectSpecRef, bool overwriteEffectLevel, int effectLevel)
        {
            var effectSpec = effectSpecRef.Value;
            var level = overwriteEffectLevel ? effectLevel : source.Level;
            effectSpec.Init(source, _owner, level);
            _gameplayEffectSpecs.Add(effectSpec);
            effectSpec.TriggerOnAdd();
            effectSpec.Apply();

            // If the gameplay effect was removed immediately after being applied, return false
            if (effectSpec.IsApplied == false)
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    $"GameplayEffect {effectSpec.GameplayEffect.GameplayEffectName} was removed immediately after being applied. This may indicate a problem with the RemoveGameplayEffectsWithTags.");
#endif
                // No need to trigger OnGameplayEffectContainerIsDirty, it has already been triggered when it was removed.
                return;
            }

            OnGameplayEffectContainerIsDirty?.Invoke();
        }
    }
}