using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime
{
    public class AbilitySystemComponent : MonoBehaviour
    {
        public int Level { get; protected set; }

        public GameplayEffectContainer GameplayEffectContainer { get; private set; }
        

        private bool _ready;

        private void Prepare()
        {
            if (_ready) return;
            GameplayEffectContainer = new GameplayEffectContainer(this);
            _ready = true;
        }

        public void Enable()
        {
        }

        public void Disable()
        {
            DisableAllAbilities();
            ClearGameplayEffects();
        }

        private void Awake()
        {
            Prepare();
        }

        private void OnEnable()
        {
            Prepare();
            //GameplayAbilitySystem.GAS.Register(this);
            Enable();
        }

        private void OnDisable()
        {
            Disable();
            //GameplayAbilitySystem.GAS.Unregister(this);
        }

        public void Init(GameplayTag[] baseTags, Type[] attrSetTypes, int level)
        {
            Prepare();
            SetLevel(level);

            if (attrSetTypes != null)
            {
                // foreach (var attrSetType in attrSetTypes)
                //     AttributeSetContainer.AddAttributeSet(attrSetType);
            }
        }

        public void SetLevel(int level)
        {
            Level = level;
        }
        public void RemoveGameplayEffect(GameplayEffectSpec spec)
        {
            GameplayEffectContainer.RemoveGameplayEffectSpec(spec);
        }

        public GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffectSpec gameplayEffectSpec,
            AbilitySystemComponent target)
        {
            return target.AddGameplayEffect(this, gameplayEffectSpec);
        }

        public GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect, AbilitySystemComponent target)
        {
            if (gameplayEffect == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"[EX] Try To Apply a NULL GameplayEffect From {name} To {target.name}!");
#endif
                return null;
            }

            var spec = gameplayEffect.CreateSpec();
            return ApplyGameplayEffectTo(spec, target);
        }

        public GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect, AbilitySystemComponent target,
            int effectLevel)
        {
            if (gameplayEffect == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"[EX] Try To Apply a NULL GameplayEffect From {name} To {target.name}!");
#endif
                return null;
            }

            var spec = gameplayEffect.CreateSpec();
            spec.SetLevel(effectLevel);
            return ApplyGameplayEffectTo(spec, target);
        }

        public GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffectSpec gameplayEffectSpec)
        {
            return ApplyGameplayEffectTo(gameplayEffectSpec, this);
        }

        public GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect)
        {
            return ApplyGameplayEffectTo(gameplayEffect, this);
        }

        public void RemoveGameplayEffectSpec(GameplayEffectSpec gameplayEffectSpec)
        {
            GameplayEffectContainer.RemoveGameplayEffectSpec(gameplayEffectSpec);
        }

        public AbilitySpec GrantAbility(AbstractAbility ability)
        {
            return null;
        }

        public void RemoveAbility(string abilityName)
        {
        }

        public float? GetAttributeCurrentValue(string setName, string attributeShortName)
        {
            return 0;
        }

        public float? GetAttributeBaseValue(string setName, string attributeShortName)
        {
            return 0;
        }

        public void Tick()
        {
            GameplayEffectContainer.Tick();
        }

        public Dictionary<string, float> DataSnapshot()
        {
            return new Dictionary<string, float>(); //AttributeSetContainer.Snapshot());
        }

        public bool TryActivateAbility(string abilityName, params object[] args)
        {
            return false;
        }

        public void TryEndAbility(string abilityName)
        {
        }

        public void TryCancelAbility(string abilityName)
        {
        }

        public void ApplyModFromInstantGameplayEffect(GameplayEffectSpec spec)
        {
            foreach (var modifier in spec.Modifiers)
            {
                // var attributeValue = GetAttributeAttributeValue(modifier.AttributeSetName, modifier.AttributeShortName);
                // if (attributeValue == null) continue;
                // if (attributeValue.Value.IsSupportOperation(modifier.Operation) == false)
                // {
                //     throw new InvalidOperationException("Unsupported operation.");
                // }
                //
                // if (attributeValue.Value.CalculateMode != CalculateMode.Stacking)
                // {
                //     throw new InvalidOperationException(
                //         $"[EX] Instant GameplayEffect Can Only Modify Stacking Mode Attribute! " +
                //         $"But {modifier.AttributeSetName}.{modifier.AttributeShortName} is {attributeValue.Value.CalculateMode}");
                // }
                //
                // var magnitude = modifier.CalculateMagnitude(spec, modifier.ModiferMagnitude);
                // var baseValue = attributeValue.Value.BaseValue;
                // switch (modifier.Operation)
                // {
                //     case GEOperation.Add:
                //         baseValue += magnitude;
                //         break;
                //     case GEOperation.Minus:
                //         baseValue -= magnitude;
                //         break;
                //     case GEOperation.Multiply:
                //         baseValue *= magnitude;
                //         break;
                //     case GEOperation.Divide:
                //         baseValue /= magnitude;
                //         break;
                //     case GEOperation.Override:
                //         baseValue = magnitude;
                //         break;
                //     default:
                //         throw new ArgumentOutOfRangeException();
                // }
                //
                // AttributeSetContainer.Sets[modifier.AttributeSetName]
                //     .ChangeAttributeBase(modifier.AttributeShortName, baseValue);
            }
        }
        
        public void ClearGameplayEffect()
        {
            // _abilityContainer = new AbilityContainer(this);
            // GameplayEffectContainer = new GameplayEffectContainer(this);
            // _attributeSetContainer = new AttributeSetContainer(this);
            // tagAggregator = new GameplayTagAggregator(this);
            GameplayEffectContainer.ClearGameplayEffect();
        }

        private GameplayEffectSpec AddGameplayEffect(AbilitySystemComponent source, GameplayEffectSpec effectSpec)
        {
            return GameplayEffectContainer.AddGameplayEffectSpec(source, effectSpec);
        }

        private GameplayEffectSpec AddGameplayEffect(AbilitySystemComponent source, GameplayEffectSpec effectSpec,
            int effectLevel)
        {
            return GameplayEffectContainer.AddGameplayEffectSpec(source, effectSpec, true, effectLevel);
        }

        private void DisableAllAbilities()
        {
        }

        private void ClearGameplayEffects()
        {
            GameplayEffectContainer.ClearGameplayEffect();
        }
    }
}