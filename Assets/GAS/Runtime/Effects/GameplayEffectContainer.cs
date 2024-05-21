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
        /// <param name="ignoreApplicationRequired">
        ///     If set to true, the method will not check whether the gameplay effect is required to be applied,
        ///     assuming that the caller has already performed this check. This can be used to optimize performance
        ///     in cases where the application requirement has been confirmed elsewhere.
        /// </param>
        /// <returns>
        ///     Returns true if the gameplay effect is successfully applied and remains active.
        ///     Returns false if the gameplay effect is applied but immediately removed due to a tag(in `AssetTags` or `GrantedTags`) match
        ///     with the `RemoveGameplayEffectsWithTags` function, indicating that the effect did not persist.
        /// </returns>
        public bool AddGameplayEffectSpec(GameplayEffectSpec spec, bool ignoreApplicationRequired = false)
        {
            if (!ignoreApplicationRequired && !spec.GameplayEffect.CanApplyTo(_owner))
            {
                return false;
            }

            if (spec.GameplayEffect.IsImmune(_owner))
            {
                spec.TriggerOnImmunity();
                return false;
            }

            if (spec.GameplayEffect.DurationPolicy == EffectsDurationPolicy.Instant)
            {
                spec.TriggerOnExecute();
            }
            else
            {
                _gameplayEffectSpecs.Add(spec);
                spec.TriggerOnAdd();
                spec.Apply();

                // If the gameplay effect was removed immediately after being applied, return false
                if (!_gameplayEffectSpecs.Contains(spec))
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.LogWarning(
                        $"GameplayEffect {spec.GameplayEffect.GameplayEffectName} was removed immediately after being applied. This may indicate a problem with the RemoveGameplayEffectsWithTags.");
#endif
                    // No need to trigger OnGameplayEffectContainerIsDirty, it has already been triggered when it was removed.
                    return false;
                }
                
                OnGameplayEffectContainerIsDirty?.Invoke();
            }

            return true;
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
    }
}