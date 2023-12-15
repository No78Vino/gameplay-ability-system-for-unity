using System;
using System.Collections.Generic;
using GAS.Core;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.AbilitySystemComponent
{
    public class AbilitySystemComponent: MonoBehaviour,IAbilitySystemComponent
    {
        public float Level{ get; private set; }

        private List<GameplayTag> _tags;


        private void OnEnable()
        {
            GameplayAbilitySystem.GAS.Register(this);
        }
        
        private void OnDisable()
        {
            GameplayAbilitySystem.GAS.Unregister(this);
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

        public void Tick()
        {
            // TODO
        }

        public GameplayEffectSpec CreateGameplayEffectSpec(GameplayEffect gameplayEffect, float level = 1f)
        {
            Level = level;
            return new GameplayEffectSpec(gameplayEffect, this, level);
        }
        
        public void AddTag(GameplayTag tag)
        {
            if(_tags.Contains(tag)) return;
            _tags.Add(tag);
        }
        
        public bool RemoveTag(GameplayTag tag)
        {
            return _tags.Remove(tag);
        }
    }
}