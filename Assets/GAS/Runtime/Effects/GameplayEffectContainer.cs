using System.Collections.Generic;
using GAS.Runtime.Component;
using GAS.Runtime.Tags;

namespace GAS.Runtime.Effects
{
    public class GameplayEffectContainer
    {
        private AbilitySystemComponent _owner;

        private List<GameplayEffectSpec> _gameplayEffectSpecs = new List<GameplayEffectSpec>();
        private List<GameplayEffectSpec> _activeGameplayEffects = new List<GameplayEffectSpec>();
        
        
        public GameplayEffectContainer(AbilitySystemComponent owner)
        {
            _owner = owner;
        }
        
        public void Tick()
        {
            foreach (var gameplayEffectSpec in _activeGameplayEffects)
            {
                gameplayEffectSpec.Tick();
            }
        }

        public void AddGameplayEffectSpec(GameplayEffectSpec spec)
        {
            if (spec.GameplayEffect.DurationPolicy == EffectsDurationPolicy.Instant)
            {
                spec.TriggerOnExecute();
            }
            else
            {
                if (spec.CanRunning())
                {
                    spec.Activate();
                }
                else
                {
                    spec.Deactivate();
                }

                _gameplayEffectSpecs.Add(spec);
                spec.TriggerOnAdd();
            }
        }

        public void RemoveGameplayEffectSpec(GameplayEffectSpec spec)
        {
            spec.Deactivate();
            spec.TriggerOnRemove();
            
            _activeGameplayEffects.Remove(spec);
            _gameplayEffectSpecs.Remove(spec);
        }

        public void RefreshGameplayEffectState()
        {
            // new active gameplay effects
            foreach (var gameplayEffectSpec in _gameplayEffectSpecs)
            {
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
        }

        void InternalExecuteMod()
        {
            // TODO
        }

        void ApplyModToAttribute()
        {
            // TODO
        }
        
        void SetAttributeBaseValue()
        {
            // TODO
            _owner.GetAttribute("Health", "Health").SetBaseValue(100);
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
    }
}