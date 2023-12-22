using System;
using System.Collections.Generic;
using GAS.Core;
using GAS.Runtime.Ability;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Effects;
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Component
{
    public class AbilitySystemComponent : MonoBehaviour, IAbilitySystemComponent
    {
        GameplayTagContainer _tags;
        public float Level { get; private set; }
        
        Dictionary<string,AbilitySpec> _abilities = new();
        AttributeSetContainer _attributeSetContainer = new();
        GameplayEffectContainer _gameplayEffectContainer;
        public GameplayEffectContainer GameplayEffectContainer => _gameplayEffectContainer;
        public GameplayTagContainer Tags => _tags;

        private void Awake()
        {
            _gameplayEffectContainer = new GameplayEffectContainer(this);
        }

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
        
        public bool HasAllTags(GameplayTagSet tags)
        {
            return _tags.HasAllTags(tags);
        }

        public bool HasAnyTags(GameplayTagSet tags)
        {
            return _tags.HasAnyTags(tags);
        }

        public GameplayEffectSpec AddGameplayEffect(GameplayEffectSpec spec)
        {
            if (spec.GameplayEffect.DurationPolicy == EffectsDurationPolicy.Instant)
            {
                ApplyInstantGameplayEffect(spec);
            }
            else
            {
                ApplyDurationalGameplayEffect(spec);
            }

            return spec;
        }
        
        public GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect,AbilitySystemComponent target)
        {
            if (gameplayEffect.CanApplyTo(target))
            {
                return target.AddGameplayEffect(gameplayEffect.CreateSpec(this,target,Level));
            }

            return null;
        }
        
        public GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect)
        {
            return ApplyGameplayEffectTo(gameplayEffect, this);
        }
        

        public void RemoveGameplayEffect(GameplayEffectSpec spec)
        {
            // TODO
        }

        public void GrantAbility(string abilityName, AbstractAbility ability)
        {
            if (_abilities.ContainsKey(abilityName)) return;
            _abilities.Add(abilityName, ability.CreateSpec(this));
        }
        
        public void RemoveAbility(string abilityName)
        {
            _abilities.Remove(abilityName);
        }
        
        public void Tick()
        {
            _gameplayEffectContainer.Tick();

            foreach (var kv in _abilities)
            {
                kv.Value.Tick();
            }
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
        
        
        
        
        
        
        
        /// <summary>
        /// Instant GameplayEffect can only trigger the method 'TriggerOnExecute' once.
        /// Instant GameplayEffect can only change the BASE VALUE.
        /// </summary>
        /// <param name="spec"></param>
        void ApplyInstantGameplayEffect(GameplayEffectSpec spec)
        {
            // TODO
            
            _attributeSetContainer.ApplyModFromGameplayEffectSpec(spec);
            spec.TriggerOnExecute();
            
            // for (var i = 0; i < spec.GameplayEffect.Modifiers.Length; i++)
            // {
            //     var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
            //     var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec, modifier.Multiplier)).GetValueOrDefault();
            //     var attribute = modifier.Attribute;
            //
            //     // If attribute doesn't exist on this character, continue to next attribute
            //     if (attribute == null) continue;
            //     this.AttributeSystem.GetAttributeValue(attribute, out var attributeValue);
            //
            //     switch (modifier.ModifierOperator)
            //     {
            //         case EAttributeModifier.Add:
            //             attributeValue.BaseValue += magnitude;
            //             break;
            //         case EAttributeModifier.Multiply:
            //             attributeValue.BaseValue *= magnitude;
            //             break;
            //         case EAttributeModifier.Override:
            //             attributeValue.BaseValue = magnitude;
            //             break;
            //     }
            //     _attributeSetContainer.(attribute, attributeValue.BaseValue);
            // }
            // spec.GameplayEffect.TriggerOnExecute();
        }

        /// <summary>
        /// Durational GameplayEffect can only change the CURRENT VALUE.
        /// </summary>
        /// <param name="spec"></param>
        void ApplyDurationalGameplayEffect(GameplayEffectSpec spec)
        {
            // TODO
            
            GameplayEffectContainer.AddGameplayEffectSpec(spec);
            
            // GameplayEffectSpec.ModifierContainer[] modifiersToApply = new GameplayEffectSpec.ModifierContainer[spec.GameplayEffect.gameplayEffect.Modifiers.Length];
            // for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            // {
            //     var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
            //     var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec, modifier.Multiplier)).GetValueOrDefault();
            //     var attributeModifier = new AttributeModifier();
            //     switch (modifier.ModifierOperator)
            //     {
            //         case EAttributeModifier.Add:
            //             attributeModifier.Add = magnitude;
            //             break;
            //         case EAttributeModifier.Multiply:
            //             attributeModifier.Multiply = magnitude;
            //             break;
            //         case EAttributeModifier.Override:
            //             attributeModifier.Override = magnitude;
            //             break;
            //     }
            //     modifiersToApply[i] = new() { Attribute = modifier.Attribute, Modifier = attributeModifier };
            // }
            // spec.modifiers = modifiersToApply;
            // spec.RaiseOnApplyEvent();
            // HandleRemoveGameplayEffectsWithTag(spec);
            // AppliedGameplayEffects.Add(spec);
        }
    }
}