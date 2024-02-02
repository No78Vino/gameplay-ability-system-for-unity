using System;
using System.Collections.Generic;
using GAS.Runtime.Component;
using GAS.Runtime.Tags;

namespace GAS.Runtime.Effects
{
    public class GameplayEffectContainer
    {
        private AbilitySystemComponent _owner;

        private readonly List<GameplayEffectSpec> _gameplayEffectSpecs = new List<GameplayEffectSpec>();
        private readonly List<GameplayEffectSpec> _activeGameplayEffects = new List<GameplayEffectSpec>();
        
        private event Action onGameplayEffectContainerIsDirty;
        public void SetOwner(AbilitySystemComponent owner)
        {
            _owner = owner;
        }
        
        public List<GameplayEffectSpec> GetActiveGameplayEffects()
        {
            return _activeGameplayEffects;
        }
        
        public void Tick()
        {
            var enumerable = _activeGameplayEffects.ToArray();
            foreach (var gameplayEffectSpec in enumerable)
            {
                gameplayEffectSpec.Tick();
            }
        }

        public void RegisterOnGameplayEffectContainerIsDirty(Action action)
        {
            onGameplayEffectContainerIsDirty += action;
        }
        
        public void UnregisterOnGameplayEffectContainerIsDirty(Action action)
        {
            onGameplayEffectContainerIsDirty -= action;
        }
        
        public void RemoveGameplayEffectWithAnyTags(GameplayTagSet tags)
        {
            if(tags.Empty) return;
            
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
                if (!grantedTags.Empty && grantedTags.HasAnyTags(tags))
                {
                    removeList.Add(gameplayEffectSpec);
                }
            }

            foreach (var gameplayEffectSpec in removeList)
            {
                RemoveGameplayEffectSpec(gameplayEffectSpec);
            }
        }
        
        /// <summary>
        /// </summary>
        /// <param name="spec"></param>
        /// <returns>
        /// If the added effect is an instant effect,return false.
        /// If the added effect is a duration effect and activate successfully ,return true.
        /// </returns>
        public bool AddGameplayEffectSpec(GameplayEffectSpec spec)
        {
            // Check Immunity Tags
            if(_owner.HasAnyTags(spec.GameplayEffect.TagContainer.ApplicationImmunityTags))
            {
                spec.TriggerOnImmunity();
                return false;
            }
            
            if (spec.GameplayEffect.DurationPolicy == EffectsDurationPolicy.Instant)
            {
                spec.TriggerOnExecute();
                return false;
            }

            _gameplayEffectSpecs.Add(spec);
            spec.TriggerOnAdd();
            
            var canRunning = spec.CanRunning();
            if (canRunning)
            {
                _activeGameplayEffects.Add(spec);
                spec.Apply();
            }
            else
            {
                spec.DisApply();
            }
            
            onGameplayEffectContainerIsDirty?.Invoke();
            
            return canRunning;
        }

        public void RemoveGameplayEffectSpec(GameplayEffectSpec spec)
        {
            spec.DisApply(); 
            spec.TriggerOnRemove();  
            _activeGameplayEffects.Remove(spec);
            _gameplayEffectSpecs.Remove(spec);

            onGameplayEffectContainerIsDirty?.Invoke();
        }

        public void RefreshGameplayEffectState()
        {
            // new active gameplay effects
            foreach (var gameplayEffectSpec in _gameplayEffectSpecs)
            {
                if(!gameplayEffectSpec.IsApplied) continue;
                if(gameplayEffectSpec.IsActive) continue;
                if (!gameplayEffectSpec.CanRunning()) continue;
                gameplayEffectSpec.Activate();
                _activeGameplayEffects.Add(gameplayEffectSpec);
            }
            
            // remove deactive gameplay effects from active list
            foreach (var gameplayEffectSpec in _activeGameplayEffects)
            {
                if (!gameplayEffectSpec.IsActive) continue;
                if (gameplayEffectSpec.CanRunning()) continue;
                gameplayEffectSpec.Deactivate();
                _activeGameplayEffects.Remove(gameplayEffectSpec);
            }
            
            onGameplayEffectContainerIsDirty?.Invoke();
        }

        public CooldownTimer CheckCooldownFromTags(GameplayTagSet tags)
        {
            float longestCooldown = 0;
            float maxDuration = 0;
            
            // Check if the cooldown tag is granted to the player, and if so, capture the remaining duration for that tag
            foreach (var spec in _activeGameplayEffects)
            {
                var grantedTags = spec.GameplayEffect.TagContainer.GrantedTags;
                if (grantedTags.Empty) continue;
                foreach (var t in grantedTags.Tags)
                {
                    foreach (var targetTag in tags.Tags)
                    {
                        if (t != targetTag) continue;
                        // If this is an infinite GE, then return null to signify this is on CD
                        if (spec.GameplayEffect.DurationPolicy ==
                            EffectsDurationPolicy.Infinite)
                        {
                            return new CooldownTimer { TimeRemaining = -1, Duration = 0 };
                        }

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
            
            _activeGameplayEffects.Clear();
            _gameplayEffectSpecs.Clear();
            
            onGameplayEffectContainerIsDirty?.Invoke();
        }
    }
}