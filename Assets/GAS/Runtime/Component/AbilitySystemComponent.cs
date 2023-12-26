using System.Collections.Generic;
using GAS.Core;
using GAS.Runtime.Ability;
using GAS.Runtime.Attribute;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Effects;
using GAS.Runtime.Effects.Modifier;
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Component
{
    public class AbilitySystemComponent : MonoBehaviour, IAbilitySystemComponent
    {
        GameplayTagContainer _tags;
        public float Level { get; private set; }
        
        Dictionary<string,AbilitySpec> _abilities = new Dictionary<string,AbilitySpec>();
        AttributeSetContainer _attributeSetContainer = new AttributeSetContainer();
        GameplayEffectContainer _gameplayEffectContainer;
        public GameplayEffectContainer GameplayEffectContainer => _gameplayEffectContainer;
        //public AttributeSetContainer AttributeSetContainer => _attributeSetContainer;

        delegate void TagIsDirty();
        event TagIsDirty OnTagIsDirty;
        
        private void Awake()
        {
            _gameplayEffectContainer = new GameplayEffectContainer(this);
        }

        private void OnEnable()
        {
            GameplayAbilitySystem.GAS.Register(this);
            OnTagIsDirty+=_gameplayEffectContainer.RefreshGameplayEffectState;
        }

        private void OnDisable()
        {
            GameplayAbilitySystem.GAS.Unregister(this);
            OnTagIsDirty-=_gameplayEffectContainer.RefreshGameplayEffectState;
        }

        public void Init()
        {
            // _abilities.Add(ability.Name, ability.CreateSpec(this));
        }
        
        public bool HasAllTags(GameplayTagSet tags)
        {
            return _tags.HasAllTags(tags);
        }

        public bool HasAnyTags(GameplayTagSet tags)
        {
            return _tags.HasAnyTags(tags);
        }

        public void AddTags(GameplayTagSet tags)
        {
            _tags.AddTag(tags);
            if(!tags.Empty)
            {
                OnTagIsDirty?.Invoke();
            }
        }

        public void RemoveTags(GameplayTagSet tags)
        {
            _tags.RemoveTag(tags);
            if(!tags.Empty)
            {
                OnTagIsDirty?.Invoke();
            }
        }

        private GameplayEffectSpec AddGameplayEffect(GameplayEffectSpec spec)
        {
            // if (spec.GameplayEffect.DurationPolicy == EffectsDurationPolicy.Instant)
            // {
            //     ApplyInstantGameplayEffect(spec);
            // }
            // else
            // {
            //     ApplyDurationalGameplayEffect(spec);
            // }
            GameplayEffectContainer.AddGameplayEffectSpec(spec);
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

        public void GrantAbility(AbstractAbility ability)
        {
            if (_abilities.ContainsKey(ability.Name)) return;
            var abilitySpec = ability.CreateSpec(this);
            _abilities.Add(ability.Name, abilitySpec);
            _tags.AddTag(abilitySpec.Ability.tag.ActivationOwnedTag);
        }

        public void RemoveAbility(string abilityName)
        {
            var abilitySpec = _abilities[abilityName];
            if(abilitySpec==null) return;
            
            _tags.RemoveTag(abilitySpec.Ability.tag.ActivationOwnedTag);
            _abilities.Remove(abilityName);
            
        }

        public AttributeBase GetAttribute(string setName,string attributeName)
        {
            return _attributeSetContainer.GetAttribute(setName, attributeName);
        }

        public void Tick()
        {
            _gameplayEffectContainer.Tick();

            foreach (var kv in _abilities)
            {
                kv.Value.Tick();
            }
        }

        public Dictionary<string, float> DataSnapshot()
        {
            return _attributeSetContainer.Snapshot();
        }

        public bool TryActivateAbility(string abilityName,params object[] args)
        {
            if (!_abilities.ContainsKey(abilityName)) return false;
            _abilities[abilityName].TryActivateAbility(args);
            return true;
        }
        
        public void EndAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;
            _abilities[abilityName].EndAbility();
        }

        public void ApplyModFromDurationalGameplayEffect(GameplayEffectSpec spec)
        {
            // TODO
            // foreach (var modifier in spec.GameplayEffect.Modifiers)
            // {
            //     var attribute = GetAttribute(modifier.AttributeSetName, modifier.AttributeShortName);
            //     if (attribute == null) continue;
            //     var magnitude = modifier.MMC.CalculateMagnitude(modifier.ModiferMagnitude);
            //     var currentValue = attribute.CurrentValue;
            //     switch (modifier.Operation)
            //     {
            //         case GEOperation.Add:
            //             currentValue += magnitude;
            //             break;
            //         case GEOperation.Multiply:
            //             currentValue *= magnitude;
            //             break;
            //         case GEOperation.Override:
            //             currentValue = magnitude;
            //             break;
            //     }
            //     _attributeSets[modifier.AttributeSetName].ChangeAttribute(attribute, currentValue);
            // }
            
            //_appliedGameplayEffectSpecs.Add(spec);
        }
        
        public void ApplyModFromInstantGameplayEffect(GameplayEffectSpec spec)
        {
            foreach (var modifier in spec.GameplayEffect.Modifiers)
            {
                var attribute = GetAttribute(modifier.AttributeSetName, modifier.AttributeShortName);
                if (attribute == null) continue;
                var magnitude = modifier.MMC.CalculateMagnitude(modifier.ModiferMagnitude);
                var baseValue = attribute.BaseValue;
                switch (modifier.Operation)
                {
                    case GEOperation.Add:
                        baseValue += magnitude;
                        break;
                    case GEOperation.Multiply:
                        baseValue *= magnitude;
                        break;
                    case GEOperation.Override:
                        baseValue = magnitude;
                        break;
                }

                _attributeSetContainer.GetAttribute(modifier.AttributeSetName, modifier.AttributeShortName);
                _attributeSetContainer.Sets[modifier.AttributeSetName].ChangeAttributeBase(attribute, baseValue);
            }
        }
        
        
        public void RemoveModFromDurationalGameplayEffect(GameplayEffectSpec spec)
        {
            _attributeSetContainer.RemoveModFromGameplayEffectSpec(spec);
        }
        
        public CooldownTimer CheckCooldownFromTags(GameplayTagSet tags)
        {
            return _gameplayEffectContainer.CheckCooldownFromTags(tags);
        }
    }
}