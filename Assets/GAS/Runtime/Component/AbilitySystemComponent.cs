using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace GAS.Runtime
{
    public class AbilitySystemComponent : MonoBehaviour, IAbilitySystemComponent
    {
        [SerializeField]
        private AbilitySystemComponentPreset preset;

        public AbilitySystemComponentPreset Preset => preset;

        public int Level { get; protected set; }

        public GameplayEffectContainer GameplayEffectContainer { get; private set; }

        public GameplayTagAggregator GameplayTagAggregator { get; private set; }

        public AbilityContainer AbilityContainer { get; private set; }

        public AttributeSetContainer AttributeSetContainer { get; private set; }

        private bool _ready;

        private void Prepare()
        {
            if (_ready) return;
            AbilityContainer = new AbilityContainer(this);
            GameplayEffectContainer = new GameplayEffectContainer(this);
            AttributeSetContainer = new AttributeSetContainer(this);
            GameplayTagAggregator = new GameplayTagAggregator(this);
            _ready = true;
        }

        public void Dispose()
        {
            DisposeAttributeSetContainer();
            DisableAllAbilities();
            ClearGameplayEffects();
            GameplayTagAggregator?.OnDisable();
        }

        private void Awake()
        {
            Prepare();
        }

        private void OnEnable()
        {
            Profiler.BeginSample($"{nameof(AbilitySystemComponent)}::OnEnable()");
            Prepare();
            GameplayAbilitySystem.GAS.Register(this);
            GameplayTagAggregator?.OnEnable();
            Profiler.EndSample();
        }

        private void OnDisable()
        {
            Dispose();
            GameplayAbilitySystem.GAS.Unregister(this);
        }

        public void SetPreset(AbilitySystemComponentPreset ascPreset)
        {
            preset = ascPreset;
        }

        public void Init(GameplayTag[] baseTags, Type[] attrSetTypes, AbilityAsset[] baseAbilities, int level)
        {
            Prepare();
            SetLevel(level);
            if (baseTags != null) GameplayTagAggregator.Init(baseTags);

            if (attrSetTypes != null)
                foreach (var attrSetType in attrSetTypes)
                    AttributeSetContainer.AddAttributeSet(attrSetType);

            if (baseAbilities != null)
                foreach (var info in baseAbilities)
                    if (info != null)
                    {
                        var ability = Activator.CreateInstance(info.AbilityType(), args: info) as AbstractAbility;
                        AbilityContainer.GrantAbility(ability);
                    }
        }

        public void SetLevel(int level)
        {
            Level = level;
        }

        public bool HasTag(GameplayTag gameplayTag)
        {
            return GameplayTagAggregator.HasTag(gameplayTag);
        }

        public bool HasAllTags(GameplayTagSet tags)
        {
            return GameplayTagAggregator.HasAllTags(tags);
        }

        public bool HasAnyTags(GameplayTagSet tags)
        {
            return GameplayTagAggregator.HasAnyTags(tags);
        }

        public void AddFixedTags(GameplayTagSet tags)
        {
            GameplayTagAggregator.AddFixedTag(tags);
        }

        public void RemoveFixedTags(GameplayTagSet tags)
        {
            GameplayTagAggregator.RemoveFixedTag(tags);
        }

        public void AddFixedTag(GameplayTag gameplayTag)
        {
            GameplayTagAggregator.AddFixedTag(gameplayTag);
        }

        public void RemoveFixedTag(GameplayTag gameplayTag)
        {
            GameplayTagAggregator.RemoveFixedTag(gameplayTag);
        }

        public void RemoveGameplayEffect(GameplayEffectSpec spec)
        {
            GameplayEffectContainer.RemoveGameplayEffectSpec(spec);
        }


        public GameplayEffectSpec ApplyGameplayEffectTo(GameplayEffect gameplayEffect, AbilitySystemComponent target)
        {
#if UNITY_EDITOR
            if (gameplayEffect == null)
            {
                Debug.LogError($"[EX] Try To Apply a NULL GameplayEffect From {name} To {target.name}!");
                return null;
            }
#endif
            Profiler.BeginSample("ApplyGameplayEffectTo()");

            Profiler.BeginSample("gameplayEffect.CanApplyTo()");
            var canApply = gameplayEffect.CanApplyTo(target);
            Profiler.EndSample();

            GameplayEffectSpec applyGameplayEffectTo = null;
            if (canApply)
            {
                var spec = gameplayEffect.CreateSpec(this, target, Level);

                Profiler.BeginSample("AddGameplayEffect()");
                applyGameplayEffectTo = target.AddGameplayEffect(spec);
                Profiler.EndSample();
            }

            Profiler.EndSample();
            return applyGameplayEffectTo;
        }

        public GameplayEffectSpec ApplyGameplayEffectToSelf(GameplayEffect gameplayEffect)
        {
            return ApplyGameplayEffectTo(gameplayEffect, this);
        }

        public AbilitySpec GrantAbility(AbstractAbility ability)
        {
            AbilityContainer.GrantAbility(ability);
            // GE的Granted Ability的Grab处理
            GameplayEffectContainer.TryGrabGrantedAbility(ability.Name);
            
            return AbilityContainer.AbilitySpecs()[ability.Name];
        }

        public void RemoveAbility(string abilityName)
        {
            // GE的Granted Ability的UnGrab处理
            GameplayEffectContainer.TryUngrabGrantedAbility(abilityName);
            
            AbilityContainer.RemoveAbility(abilityName);
        }

        public float? GetAttributeCurrentValue(string setName, string attributeShortName)
        {
            var value = AttributeSetContainer.GetAttributeCurrentValue(setName, attributeShortName);
            return value;
        }

        public float? GetAttributeBaseValue(string setName, string attributeShortName)
        {
            var value = AttributeSetContainer.GetAttributeBaseValue(setName, attributeShortName);
            return value;
        }

        public void Tick()
        {
            Profiler.BeginSample($"{nameof(AbilitySystemComponent)}::Tick()");
            AbilityContainer.Tick();
            GameplayEffectContainer.Tick();
            Profiler.EndSample();
        }

        public Dictionary<string, float> DataSnapshot()
        {
            return AttributeSetContainer.Snapshot();
        }

        public bool TryActivateAbility(string abilityName, params object[] args)
        {
            return AbilityContainer.TryActivateAbility(abilityName, args);
        }

        public void TryEndAbility(string abilityName)
        {
            AbilityContainer.EndAbility(abilityName);
        }

        public void TryCancelAbility(string abilityName)
        {
            AbilityContainer.CancelAbility(abilityName);
        }

        public void ApplyModFromInstantGameplayEffect(GameplayEffectSpec spec)
        {
            foreach (var modifier in spec.GameplayEffect.Modifiers)
            {
                var attributeBaseValue = GetAttributeBaseValue(modifier.AttributeSetName, modifier.AttributeShortName);
                if (attributeBaseValue == null) continue;
                var magnitude = modifier.CalculateMagnitude(spec, modifier.ModiferMagnitude);
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                AttributeSetContainer.Sets[modifier.AttributeSetName]
                    .ChangeAttributeBase(modifier.AttributeShortName, baseValue);
            }
        }

        public CooldownTimer CheckCooldownFromTags(GameplayTagSet tags)
        {
            return GameplayEffectContainer.CheckCooldownFromTags(tags);
        }

        public T AttrSet<T>() where T : AttributeSet
        {
            AttributeSetContainer.TryGetAttributeSet<T>(out var attrSet);
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

        private GameplayEffectSpec AddGameplayEffect(GameplayEffectSpec spec)
        {
            var success = GameplayEffectContainer.AddGameplayEffectSpec(spec);
            return success ? spec : null;
        }

        private void DisableAllAbilities()
        {
            AbilityContainer.CancelAllAbilities();
        }

        private void ClearGameplayEffects()
        {
            GameplayEffectContainer.ClearGameplayEffect();
        }

        private void DisposeAttributeSetContainer()
        {
            AttributeSetContainer.Dispose();
        }
    }
}