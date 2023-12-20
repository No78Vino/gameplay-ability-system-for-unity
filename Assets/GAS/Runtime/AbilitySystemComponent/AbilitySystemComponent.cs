using System.Collections.Generic;
using GAS.Core;
using GAS.Runtime.Ability;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.AbilitySystemComponent
{
    public class AbilitySystemComponent : MonoBehaviour, IAbilitySystemComponent
    {
        private GameplayTagContainer _tags;
        public float Level { get; private set; }

        List<GameplayEffectSpec> _activeGameplayEffects = new();
        Dictionary<string,AbilitySpec> _abilities = new();

        public List<GameplayEffectSpec> GetActiveGameplayEffects() => _activeGameplayEffects;
        
        private void OnEnable()
        {
            GameplayAbilitySystem.GAS.Register(this);
        }

        private void OnDisable()
        {
            GameplayAbilitySystem.GAS.Unregister(this);
        }

        public void Init(AbstractAbility ability)
        {
            _abilities.Add(ability.Name, ability.CreateSpec(this));
        }
        public bool HasAllTags(List<GameplayTag> tags)
        {
            if (tags.Count == 0) return true;
            // TODO
            // Check ASC Has All Tags
            return false;
        }

        public bool HasAnyTags(List<GameplayTag> tags)
        {
            if (tags.Count == 0) return true;
            // TODO
            return false;
        }

        public void ApplyGameplayEffectToSelf(GameplayEffectSpec spec)
        {
            // TODO
        }

        public void RemoveActiveGameplayEffect(GameplayEffectSpec spec)
        {
            // TODO
        }

        public void AddAbility(string abilityName, AbstractAbility ability)
        {
            if (_abilities.ContainsKey(abilityName)) return;
            _abilities.Add(abilityName, ability.CreateSpec(this));
        }
        
        public void Tick()
        {
            foreach (var ge in _activeGameplayEffects)
            {
                ge.Tick();
            }

            foreach (var kv in _abilities)
            {
                kv.Value.Tick();
            }
        }

        public GameplayEffectSpec CreateGameplayEffectSpec(GameplayEffect gameplayEffect, float level = 1f)
        {
            Level = level;
            return new GameplayEffectSpec(gameplayEffect, this, level);
        }

        public void AddTag(GameplayTag tag)
        {
            _tags.AddTag(tag);
        }

        public void RemoveTag(GameplayTag tag)
        {
            _tags.RemoveTag(tag);
        }
        
        public bool TryActivateAbility(string abilityName,params object[] args)
        {
            if (!_abilities.ContainsKey(abilityName)) return false;
            _abilities[abilityName].ActivateAbility(args);
            return true;
        }
        
        public void TryEndAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;
            _abilities[abilityName].EndAbility();
        }
    }
}