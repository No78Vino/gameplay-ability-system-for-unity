using System;
using System.Collections.Generic;
using GAS.General;
using UnityEngine;

namespace GAS.Runtime
{
    public class AttributeAggregator
    {
        private record ModifierSpec
        {
            public EntityRef<GameplayEffectSpec> SpecRef { get; private set; }
            public GameplayEffectModifier Modifier { get; private set; }

            public void Init(EntityRef<GameplayEffectSpec> spec, GameplayEffectModifier modifier)
            {
                SpecRef = spec;
                Modifier = modifier;
            }

            public void Release()
            {
                SpecRef = default;
                Modifier = default;
            }
        }

        AttributeBase _processedAttribute;
        AbilitySystemComponent _owner;

        /// <summary>
        ///  modifiers的顺序很重要，因为modifiers的执行是按照顺序来的。
        /// </summary>
        private readonly List<ModifierSpec> _modifierCache = new();

        public AttributeAggregator(AttributeBase attribute, AbilitySystemComponent owner)
        {
            _processedAttribute = attribute;
            _owner = owner;

            // OnEnable();
        }

        public void OnEnable()
        {
            _processedAttribute.RegisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueIsDirty);
            _owner.GameplayEffectContainer.RegisterOnGameplayEffectContainerIsDirty(RefreshModifierCache);
        }

        public void OnDisable()
        {
            _processedAttribute.UnregisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueIsDirty);
            _owner.GameplayEffectContainer.UnregisterOnGameplayEffectContainerIsDirty(RefreshModifierCache);
        }

        public void OnDestroy()
        {
            ReleaseModifiersCache();
        }

        /// <summary>
        /// it's triggered only when the owner's gameplay effect is added or removed. 
        /// </summary>
        void RefreshModifierCache()
        {
            // UnityEngine.Profiling.Profiler.BeginSample("AttributeAggregator.RefreshModifierCache");

            // 注销属性变化监听回调
            UnregisterAttributeChangedListen();
            ReleaseModifiersCache();

            var gameplayEffects = _owner.GameplayEffectContainer.GameplayEffects();
            foreach (var geSpec in gameplayEffects)
            {
                if (geSpec.IsActive)
                {
                    foreach (var modifier in geSpec.Modifiers)
                    {
                        if (modifier.AttributeName == _processedAttribute.Name)
                        {
                            var modifierSpec = ObjectPool.Instance.Fetch<ModifierSpec>();
                            modifierSpec.Init(geSpec, modifier);
                            _modifierCache.Add(modifierSpec);
                            TryRegisterAttributeChangedListen(geSpec, modifier);
                        }
                    }
                }
            }

            UpdateCurrentValueWhenModifierIsDirty();

            // UnityEngine.Profiling.Profiler.EndSample();
        }

        private void ReleaseModifiersCache()
        {
            foreach (var modifierSpec in _modifierCache)
            {
                modifierSpec.Release();
                ObjectPool.Instance.Recycle(modifierSpec);
            }

            _modifierCache.Clear();
        }

        /// <summary>
        /// 为CurrentValue计算新值。 (BaseValue的变化依赖于instant型GameplayEffect.)
        /// 这个方法的触发时机为：
        /// 1._modifierCache变化时
        /// 2._processedAttribute的BaseValue变化时
        /// 3._modifierCache的AttributeBased类的MMC，Track类属性变化时
        /// </summary>
        /// <returns></returns>
        float CalculateNewValue()
        {
            switch (_processedAttribute.CalculateMode)
            {
                case CalculateMode.Stacking:
                {
                    float newValue = _processedAttribute.BaseValue;
                    foreach (var modifierSpec in _modifierCache)
                    {
                        var spec = modifierSpec.SpecRef;
                        var modifier = modifierSpec.Modifier;
                        var magnitude = modifier.CalculateMagnitude(spec, modifier.ModiferMagnitude);

                        if (_processedAttribute.IsSupportOperation(modifier.Operation) == false)
                        {
                            throw new InvalidOperationException("Unsupported operation.");
                        }

                        switch (modifier.Operation)
                        {
                            case GEOperation.Add:
                                newValue += magnitude;
                                break;
                            case GEOperation.Minus:
                                newValue -= magnitude;
                                break;
                            case GEOperation.Multiply:
                                newValue *= magnitude;
                                break;
                            case GEOperation.Divide:
                                newValue /= magnitude;
                                break;
                            case GEOperation.Override:
                                newValue = magnitude;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    return newValue;
                }
                case CalculateMode.MinValueOnly:
                {
                    var hasOverride = false;
                    var min = float.MaxValue;
                    foreach (var modifierSpec in _modifierCache)
                    {
                        var spec = modifierSpec.SpecRef;
                        var modifier = modifierSpec.Modifier;

                        if (_processedAttribute.IsSupportOperation(modifier.Operation) == false)
                        {
                            throw new InvalidOperationException("Unsupported operation.");
                        }

                        if (modifier.Operation != GEOperation.Override)
                        {
                            throw new InvalidOperationException("MinValueOnly mode only support override operation.");
                        }

                        var magnitude = modifier.CalculateMagnitude(spec, modifier.ModiferMagnitude);
                        min = Mathf.Min(min, magnitude);
                        hasOverride = true;
                    }

                    return hasOverride ? min : _processedAttribute.BaseValue;
                }
                case CalculateMode.MaxValueOnly:
                {
                    var hasOverride = false;
                    var max = float.MinValue;
                    foreach (var modifierSpec in _modifierCache)
                    {
                        var spec = modifierSpec.SpecRef;
                        var modifier = modifierSpec.Modifier;

                        if (_processedAttribute.IsSupportOperation(modifier.Operation) == false)
                        {
                            throw new InvalidOperationException("Unsupported operation.");
                        }

                        if (modifier.Operation != GEOperation.Override)
                        {
                            throw new InvalidOperationException("MaxValueOnly mode only support override operation.");
                        }

                        var magnitude = modifier.CalculateMagnitude(spec, modifier.ModiferMagnitude);
                        max = Mathf.Max(max, magnitude);
                        hasOverride = true;
                    }

                    return hasOverride ? max : _processedAttribute.BaseValue;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void UpdateCurrentValueWhenBaseValueIsDirty(AttributeBase attribute, float oldBaseValue, float newBaseValue)
        {
            if (Mathf.Approximately(oldBaseValue, newBaseValue)) return;

            float newValue = CalculateNewValue();
            _processedAttribute.SetCurrentValue(newValue);
        }

        void UpdateCurrentValueWhenModifierIsDirty()
        {
            float newValue = CalculateNewValue();
            _processedAttribute.SetCurrentValue(newValue);
        }

        private void UnregisterAttributeChangedListen()
        {
            foreach (var modifierSpec in _modifierCache)
                TryUnregisterAttributeChangedListen(modifierSpec.SpecRef, modifierSpec.Modifier);
        }

        private void TryUnregisterAttributeChangedListen(GameplayEffectSpec ge, GameplayEffectModifier modifier)
        {
            if (modifier.MMC is AttributeBasedModCalculation mmc &&
                mmc.captureType == AttributeBasedModCalculation.GEAttributeCaptureType.Track)
            {
                if (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Target)
                {
                    if (ge.Owner != null)
                        ge.Owner.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
                            .UnregisterPostCurrentValueChange(OnAttributeChanged);
                }
                else
                {
                    if (ge.Source != null)
                        ge.Source.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
                            .UnregisterPostCurrentValueChange(OnAttributeChanged);
                }
            }
        }

        private void TryRegisterAttributeChangedListen(GameplayEffectSpec ge, GameplayEffectModifier modifier)
        {
            if (modifier.MMC is AttributeBasedModCalculation mmc &&
                mmc.captureType == AttributeBasedModCalculation.GEAttributeCaptureType.Track)
            {
                if (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Target)
                {
                    if (ge.Owner != null)
                        ge.Owner.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
                            .RegisterPostCurrentValueChange(OnAttributeChanged);
                }
                else
                {
                    if (ge.Source != null)
                        ge.Source.AttributeSetContainer.Sets[mmc.attributeSetName][mmc.attributeShortName]
                            .RegisterPostCurrentValueChange(OnAttributeChanged);
                }
            }
        }

        private void OnAttributeChanged(AttributeBase attribute, float oldValue, float newValue)
        {
            if (_modifierCache.Count == 0) return;
            foreach (var modifierSpec in _modifierCache)
            {
                var geSpec = modifierSpec.SpecRef.Value;
                if (geSpec == null)
                {
                    Debug.LogError("ge spec is invalid!");
                    continue;
                }
                
                var modifier = modifierSpec.Modifier;
                if (modifier.MMC is AttributeBasedModCalculation { captureType: AttributeBasedModCalculation.GEAttributeCaptureType.Track } mmc &&
                    attribute.Name == mmc.attributeName)
                {
                    if ((mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Target &&
                         attribute.Owner == geSpec.Owner) ||
                        (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Source &&
                         attribute.Owner == geSpec.Source))
                    {
                        UpdateCurrentValueWhenModifierIsDirty();
                        break;
                    }
                }
            }
        }
    }
}