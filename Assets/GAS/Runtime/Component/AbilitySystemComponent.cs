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
        private AbilityContainer _abilityContainer = new AbilityContainer();
        private AttributeSetContainer _attributeSetContainer = new AttributeSetContainer();
        private GameplayTagAggregator tagAggregator = new GameplayTagAggregator();

        public int Level { get; private set; }

        public GameplayEffectContainer GameplayEffectContainer { get; private set; } = new GameplayEffectContainer();

        private void Awake()
        {
            _abilityContainer.SetOwner(this);
            GameplayEffectContainer.SetOwner(this);
            _attributeSetContainer.SetOwner(this);
            tagAggregator.SetOwner(this);
            Init(preset);
        }

        private void OnEnable()
        {
            GameplayAbilitySystem.GAS.Register(this);
            tagAggregator.OnEnable();
        }

        private void OnDisable()
        {
            GameplayAbilitySystem.GAS.Unregister(this);
            tagAggregator.OnDisable();
        }

        public void DefaultInit()
        {
            Init(preset);
        }
        
        public void Init(AbilitySystemComponentPreset ascPreset)
        {
            if (ascPreset != null)
            {
                // Tag
                if (ascPreset.BaseTags != null)
                {
                    tagAggregator.Init(ascPreset.BaseTags);
                }

                // AttributeSet
                if (ascPreset.AttributeSets != null)
                {
                    foreach (var attributeSet in ascPreset.AttributeSets)
                    {
                        string attrSetTypeName = GasDefine.GAS_ATTRIBUTESET_CLASS_TYPE_PREFIX + attributeSet;
                        var attrSetType = Type.GetType($"{attrSetTypeName}, Assembly-CSharp");
                        if (attrSetType != null)
                        {
                            _attributeSetContainer.AddAttributeSet(attrSetType);
                        }
                    }
                }

                // Ability
                if (ascPreset.BaseAbilities != null)
                {
                    foreach (var abilityAsset in ascPreset.BaseAbilities)
                    {
                        var abilityType = Type.GetType($"{abilityAsset.InstanceAbilityClassFullName}, Assembly-CSharp");
                        if (abilityType != null)
                        {
                            var ability = Activator.CreateInstance(abilityType, args: abilityAsset) as AbstractAbility;
                            _abilityContainer.GrantAbility(ability);
                        }
                    }
                }
            }
        }
        
        public bool HasTag(GameplayTag gameplayTag)
        {
            return tagAggregator.HasTag(gameplayTag);
        }
        
        public bool HasAllTags(GameplayTagSet tags)
        {
            return tagAggregator.HasAllTags(tags);
        }

        public bool HasAnyTags(GameplayTagSet tags)
        {
            return tagAggregator.HasAnyTags(tags);
        }

        public void AddFixedTags(GameplayTagSet tags)
        {
            tagAggregator.AddFixedTag(tags);
        }

        public void RemoveFixedTags(GameplayTagSet tags)
        {
            tagAggregator.RemoveFixedTag(tags);
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
            _abilityContainer.Tick();
            GameplayEffectContainer.Tick();
        }

        public Dictionary<string, float> DataSnapshot()
        {
            return _attributeSetContainer.Snapshot();
        }
        
        public bool TryActivateAbility(string abilityName, params object[] args)
        {
            return _abilityContainer.TryActivateAbility(abilityName, args);
        }

        public void TryEndAbility(string abilityName)
        {
            _abilityContainer.EndAbility(abilityName);
        }

        public void ApplyModFromInstantGameplayEffect(GameplayEffectSpec spec)
        {
            foreach (var modifier in spec.GameplayEffect.Modifiers)
            {
                var attributeBaseValue = GetAttributeBaseValue(modifier.AttributeSetName, modifier.AttributeShortName);
                if (attributeBaseValue == null) continue;
                var magnitude = modifier.MMC.CalculateMagnitude(spec,modifier.ModiferMagnitude);
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
                    .ChangeAttributeBase(modifier.AttributeShortName, baseValue);
            }
        }

        public CooldownTimer CheckCooldownFromTags(GameplayTagSet tags)
        {
            return GameplayEffectContainer.CheckCooldownFromTags(tags);
        }

        public T AttrSet<T>() where T : AttributeSet.AttributeSet
        {
            _attributeSetContainer.TryGetAttributeSet<T>(out var attrSet);
            return attrSet;
        }

        public void ClearGameplayEffect()
        {
            // _abilityContainer = new AbilityContainer(this);
            // GameplayEffectContainer = new GameplayEffectContainer(this);
            // _attributeSetContainer = new AttributeSetContainer(this);
            // tagAggregator = new GameplayTagAggregator(this);

            GameplayEffectContainer.ClearGameplayEffect();
        }
        
        #if UNITY_EDITOR
        public GameplayTagAggregator GameplayTagAggregator => tagAggregator;
        public AbilityContainer AbilityContainer => _abilityContainer;
        public AttributeSetContainer AttributeSetContainer => _attributeSetContainer;
        #endif
    }
}