using System;
using System.Collections.Generic;
using UnityEngine;

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

        public void Enable()
        {
            AttributeSetContainer.OnEnable();
        }

        public void Disable()
        {
            AttributeSetContainer.OnDisable();
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
            Prepare();
            GameplayAbilitySystem.GAS.Register(this);
            GameplayTagAggregator?.OnEnable();
            Enable();
        }

        private void OnDisable()
        {
            Disable();
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
            {
                foreach (var attrSetType in attrSetTypes)
                    AttributeSetContainer.AddAttributeSet(attrSetType);
            }

            if (baseAbilities != null)
            {
                foreach (var info in baseAbilities)
                    GrantAbility(info);
            }
        }

        private void GrantAbility(AbilityAsset info)
        {
            if (info == null)
            {
                Debug.LogWarning($"[EX] Try To Grant a NULL Ability!");
                return;
            }

            try
            {
                var ability = Activator.CreateInstance(info.AbilityType(), args: info) as AbstractAbility;
                AbilityContainer.GrantAbility(ability);
            }
#pragma warning disable CS0168 // 声明了变量，但从未使用过
            catch (MissingMethodException e)
#pragma warning restore CS0168 // 声明了变量，但从未使用过
            {
                // 踩坑日志:
                //   复制了某个AbilityAsset实现类的代码，但忘记更新AbilityType()方法的返回值。
                //   一般来说AbilityAsset和Ability应该是配套的, 比如在"GAA_xxx"中返回"GA_xxx"的类型.
                Debug.LogError($"[EX] 创建能力失败: " +
                               $"请检查AbilityAsset实现类'{info.GetType().FullName}'中的AbilityType()方法" +
                               $"是否正确返回了能力类型(当前为'{info.AbilityType()?.FullName ?? "null"}')。");
                throw;
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

        public void RemoveGameplayEffectWithAnyTags(GameplayTagSet tags)
        {
            GameplayEffectContainer.RemoveGameplayEffectWithAnyTags(tags);
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
            AbilityContainer.GrantAbility(ability);
            return AbilityContainer.AbilitySpecs()[ability.Name];
        }

        public void RemoveAbility(string abilityName)
        {
            AbilityContainer.RemoveAbility(abilityName);
        }

        public AttributeValue? GetAttributeAttributeValue(string attrSetName, string attrShortName)
        {
            var value = AttributeSetContainer.GetAttributeAttributeValue(attrSetName, attrShortName);
            return value;
        }

        public CalculateMode? GetAttributeCalculateMode(string attrSetName, string attrShortName)
        {
            var value = AttributeSetContainer.GetAttributeCalculateMode(attrSetName, attrShortName);
            return value;
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
            AbilityContainer.Tick();
            GameplayEffectContainer.Tick();
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
            foreach (var modifier in spec.Modifiers)
            {
                var attributeValue = GetAttributeAttributeValue(modifier.AttributeSetName, modifier.AttributeShortName);
                if (attributeValue == null) continue;
                if (attributeValue.Value.IsSupportOperation(modifier.Operation) == false)
                {
                    throw new InvalidOperationException("Unsupported operation.");
                }

                if (attributeValue.Value.CalculateMode != CalculateMode.Stacking)
                {
                    throw new InvalidOperationException(
                        $"[EX] Instant GameplayEffect Can Only Modify Stacking Mode Attribute! " +
                        $"But {modifier.AttributeSetName}.{modifier.AttributeShortName} is {attributeValue.Value.CalculateMode}");
                }

                var magnitude = modifier.CalculateMagnitude(spec, modifier.ModiferMagnitude);
                var baseValue = attributeValue.Value.BaseValue;
                switch (modifier.Operation)
                {
                    case GEOperation.Add:
                        baseValue += magnitude;
                        break;
                    case GEOperation.Minus:
                        baseValue -= magnitude;
                        break;
                    case GEOperation.Multiply:
                        baseValue *= magnitude;
                        break;
                    case GEOperation.Divide:
                        baseValue /= magnitude;
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
            AbilityContainer.CancelAllAbilities();
        }

        private void ClearGameplayEffects()
        {
            GameplayEffectContainer.ClearGameplayEffect();
        }
    }
}