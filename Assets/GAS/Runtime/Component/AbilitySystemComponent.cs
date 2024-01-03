using System;
using System.Collections.Generic;
using GAS.Core;
using GAS.Runtime.Ability;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Effects;
using GAS.Runtime.Effects.Modifier;
using GAS.Runtime.Tags;
using UnityEngine;

namespace GAS.Runtime.Component
{
    public class AbilitySystemComponent : MonoBehaviour, IAbilitySystemComponent
    {
        [SerializeField] private AbilitySystemComponentPreset preset;
        private AbilityContainer _abilityContainer;
        private AttributeSetContainer _attributeSetContainer;
        private GameplayTagCollection _tagCollection;

        public float Level { get; private set; }
        
        public GameplayEffectContainer GameplayEffectContainer { get; private set; }

        private void Awake()
        {
            _abilityContainer = new AbilityContainer(this);
            GameplayEffectContainer = new GameplayEffectContainer(this);
            _attributeSetContainer = new AttributeSetContainer(this);
            _tagCollection = new GameplayTagCollection(this);
        }

        private void Start()
        {
            Init(preset);
        }

        private void OnEnable()
        {
            GameplayAbilitySystem.GAS.Register(this);
            _tagCollection.OnEnable();
        }

        private void OnDisable()
        {
            GameplayAbilitySystem.GAS.Unregister(this);
            _tagCollection.OnDisable();
        }

        public void Init(AbilitySystemComponentPreset ascPreset)
        {
            if (ascPreset != null)
            {
                // Tag
                if (ascPreset.BaseTags != null)
                {
                    foreach (var gameplayTag in ascPreset.BaseTags)
                    {
                        _tagCollection.AddTag(gameplayTag);
                    }
                }

                // AttributeSet
                if(ascPreset.AttributeSets!=null)
                {foreach (var attributeSet in ascPreset.AttributeSets)
                {
                    string attrSetTypeName = GasDefine.GAS_ATTRIBUTESET_CLASS_TYPE_PREFIX + attributeSet;
                    var attrSetType = Type.GetType(attrSetTypeName);
                    if (attrSetType != null)
                    {
                        _attributeSetContainer.AddAttributeSet(attrSetType);
                    }
                }}
                
                // Ability
                if (ascPreset.BaseAbilities != null)
                {
                    foreach (var abilityAsset in ascPreset.BaseAbilities)
                    {
                        if (!(Type.GetType(abilityAsset.InstanceAbilityClassFullName) is { } abilityType)) continue;
                        var ability = Activator.CreateInstance(abilityType) as AbstractAbility;
                        _abilityContainer.GrantAbility(ability);
                    }
                }
            }
        }
        
        public bool HasAllTags(GameplayTagSet tags)
        {
            return _tagCollection.HasAllTags(tags);
        }

        public bool HasAnyTags(GameplayTagSet tags)
        {
            return _tagCollection.HasAnyTags(tags);
        }

        public void AddTags(GameplayTagSet tags)
        {
            _tagCollection.AddTags(tags);
        }

        public void RemoveTags(GameplayTagSet tags)
        {
            _tagCollection.RemoveTags(tags);
        }

        private GameplayEffectSpec AddGameplayEffect(GameplayEffectSpec spec)
        {
            bool success = GameplayEffectContainer.AddGameplayEffectSpec(spec);
            return success ? spec : null;
        }
        
        public void RemoveGameplayEffect(GameplayEffectSpec spec)
        {
            GameplayEffectContainer.RemoveGameplayEffectSpec(spec);
        }


        public GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect, AbilitySystemComponent target)
        {
            return gameplayEffect.CanApplyTo(target)
                ? target.AddGameplayEffect(gameplayEffect.CreateSpec(this, target, Level))
                : null;
        }

        public GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect)
        {
            return ApplyGameplayEffectTo(gameplayEffect, this);
        }

        public void GrantAbility(AbstractAbility ability)
        {
            _abilityContainer.GrantAbility(ability);
        }

        public void RemoveAbility(string abilityName)
        {
            _abilityContainer.RemoveAbility(abilityName);
        }

        public float? GetAttributeCurrentValue(string setName, string attributeShortName)
        {
            var value = _attributeSetContainer.GetAttributeCurrentValue(setName, attributeShortName);
            return value;
        }

        public float? GetAttributeBaseValue(string setName, string attributeShortName)
        {
            var value = _attributeSetContainer.GetAttributeBaseValue(setName, attributeShortName);
            return value;
        }

        public void Tick()
        {
            GameplayEffectContainer.Tick();
            _abilityContainer.Tick();
        }

        public Dictionary<string, float> DataSnapshot()
        {
            return _attributeSetContainer.Snapshot();
        }
        
        public bool TryActivateAbility(string abilityName, params object[] args)
        {
            return _abilityContainer.TryActivateAbility(abilityName, args);
        }

        public void EndAbility(string abilityName)
        {
            _abilityContainer.EndAbility(abilityName);
        }


        public void ApplyModFromInstantGameplayEffect(GameplayEffectSpec spec)
        {
            foreach (var modifier in spec.GameplayEffect.Modifiers)
            {
                var attributeBaseValue = GetAttributeBaseValue(modifier.AttributeSetName, modifier.AttributeShortName);
                if (attributeBaseValue == null) continue;
                var magnitude = modifier.MMC.CalculateMagnitude(modifier.ModiferMagnitude);
                var baseValue = attributeBaseValue.Value;
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
                
                _attributeSetContainer.Sets[modifier.AttributeSetName]
                    .ChangeAttributeBase(modifier.AttributeName, baseValue);
            }
        }

        public CooldownTimer CheckCooldownFromTags(GameplayTagSet tags)
        {
            return GameplayEffectContainer.CheckCooldownFromTags(tags);
        }
    }
}