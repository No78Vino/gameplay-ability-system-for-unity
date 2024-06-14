using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public class GameplayEffectContainer
    {
        private readonly AbilitySystemComponent _owner;
        private readonly List<GameplayEffectSpec> _gameplayEffectSpecs = new List<GameplayEffectSpec>();
        private readonly List<GameplayEffectSpec> _cachedGameplayEffectSpecs = new List<GameplayEffectSpec>();

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
            _cachedGameplayEffectSpecs.AddRange(_gameplayEffectSpecs);

            foreach (var gameplayEffectSpec in _cachedGameplayEffectSpecs)
            {
                if (gameplayEffectSpec.IsActive)
                {
                    gameplayEffectSpec.Tick();
                }
            }

            _cachedGameplayEffectSpecs.Clear();
        }

        public void RegisterOnGameplayEffectContainerIsDirty(Action action)
        {
            OnGameplayEffectContainerIsDirty += action;
        }

        public void UnregisterOnGameplayEffectContainerIsDirty(Action action)
        {
            OnGameplayEffectContainerIsDirty -= action;
        }

        public void RemoveGameplayEffectWithAnyTags(GameplayTagSet tags)
        {
            if (tags.Empty) return;

            var removeList = new List<GameplayEffectSpec>();
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
        }

        /// <summary>
        /// </summary>
        /// <param name="spec"></param>
        /// <returns>
        ///     Returns true if the gameplay effect is successfully applied and remains active.
        ///     Returns false if the gameplay effect is applied but immediately removed due to a tag(in `AssetTags` or `GrantedTags`) match
        ///     with the `RemoveGameplayEffectsWithTags` function, indicating that the effect did not persist.
        /// </returns>
        public GameplayEffectSpec AddGameplayEffectSpec(AbilitySystemComponent source,GameplayEffectSpec effectSpec,bool overwriteEffectLevel = false,int effectLevel = 0)
        {
            if (!effectSpec.GameplayEffect.CanApplyTo(_owner)) return null;
            
            if (effectSpec.GameplayEffect.IsImmune(_owner))
            {
                // TODO 免疫Cue触发
                // var lv = overwriteEffectLevel ? effectLevel : source.Level;
                // effectSpec.Init(source, _owner, lv);
                // effectSpec.TriggerOnImmunity();
                return null;
            }
            
            var level = overwriteEffectLevel ? effectLevel : source.Level;
            if (effectSpec.DurationPolicy == EffectsDurationPolicy.Instant)
            {
                effectSpec.Init(source, _owner, level);
                effectSpec.TriggerOnExecute();
                return null;
            }

            // Check GE Stacking
            if (effectSpec.Stacking.stackingType == StackingType.None)
            {
                return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
            }
            
            // 处理GE堆叠
            // 基于Target类型GE堆叠
            if (effectSpec.Stacking.stackingType == StackingType.AggregateByTarget)
            {
                GetStackingEffectSpecByData(effectSpec.GameplayEffect, out var geSpec);
                // 新添加GE
                if (geSpec == null)
                    return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
                bool stackCountChange = geSpec.RefreshStack();
                if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
                return geSpec;
            }
            
            // 基于Source类型GE堆叠
            if (effectSpec.Stacking.stackingType == StackingType.AggregateBySource)
            {
                GetStackingEffectSpecByDataFrom(effectSpec.GameplayEffect,source, out var geSpec);
                if (geSpec == null)
                    return Operation_AddNewGameplayEffectSpec(source, effectSpec,overwriteEffectLevel,effectLevel);
                bool stackCountChange = geSpec.RefreshStack();
                if (stackCountChange) OnRefreshStackCountMakeContainerDirty();
                return geSpec;
            }

            return null;
        }

        public GameplayEffectSpec AddGameplayEffectSpec(AbilitySystemComponent source, GameplayEffect effect,int effectLevel)
        {
            var spec = effect.CreateSpec();
            return AddGameplayEffectSpec(source, spec, true,effectLevel);
        }
        
        public void RemoveGameplayEffectSpec(GameplayEffectSpec spec)
        {
            spec.DisApply();
            spec.TriggerOnRemove();
            _gameplayEffectSpecs.Remove(spec);

            OnGameplayEffectContainerIsDirty?.Invoke();
        }

        public void RefreshGameplayEffectState()
        {
            foreach (var gameplayEffectSpec in _gameplayEffectSpecs)
            {
                if (!gameplayEffectSpec.IsApplied) continue;
                if (!gameplayEffectSpec.IsActive)
                {
                    // new active gameplay effects
                    if (gameplayEffectSpec.GameplayEffect.CanRunning(_owner)) gameplayEffectSpec.Activate();
                }
                else
                {
                    // new deactive gameplay effects
                    if (!gameplayEffectSpec.GameplayEffect.CanRunning(_owner)) gameplayEffectSpec.Deactivate();
                }
            }

            OnGameplayEffectContainerIsDirty?.Invoke();
        }

        public CooldownTimer CheckCooldownFromTags(GameplayTagSet tags)
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
                        if (spec.GameplayEffect.DurationPolicy ==
                            EffectsDurationPolicy.Infinite)
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
            foreach (var gameplayEffectSpec in _gameplayEffectSpecs)
            {
                gameplayEffectSpec.DisApply();
                gameplayEffectSpec.TriggerOnRemove();
            }

            _gameplayEffectSpecs.Clear();

            OnGameplayEffectContainerIsDirty?.Invoke();
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

        private void GetStackingEffectSpecByDataFrom(GameplayEffect effect,AbilitySystemComponent source, 
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
        
        private GameplayEffectSpec Operation_AddNewGameplayEffectSpec(AbilitySystemComponent source,GameplayEffectSpec effectSpec,
            bool overwriteEffectLevel,int effectLevel)
        {
            var level = overwriteEffectLevel ? effectLevel : source.Level;
            effectSpec.Init(source, _owner, level);
            _gameplayEffectSpecs.Add(effectSpec);
            effectSpec.TriggerOnAdd();
            effectSpec.Apply();

            // If the gameplay effect was removed immediately after being applied, return false
            if (!_gameplayEffectSpecs.Contains(effectSpec))
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning(
                    $"GameplayEffect {effectSpec.GameplayEffect.GameplayEffectName} was removed immediately after being applied. This may indicate a problem with the RemoveGameplayEffectsWithTags.");
#endif
                // No need to trigger OnGameplayEffectContainerIsDirty, it has already been triggered when it was removed.
                return null;
            }

            OnGameplayEffectContainerIsDirty?.Invoke();
            return effectSpec;
        }
    }
}