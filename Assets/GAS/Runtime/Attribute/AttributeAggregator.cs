using System;
using System.Collections.Generic;
using UnityEngine;

namespace GAS.Runtime
{
    public class AttributeAggregator
    {
        AttributeBase _processedAttribute;
        AbilitySystemComponent _owner;

        /// <summary>
        ///  modifiers的顺序很重要，因为modifiers的执行是按照顺序来的。
        /// </summary>
        private List<Tuple<GameplayEffectSpec, GameplayEffectModifier>> _modifierCache =
            new List<Tuple<GameplayEffectSpec, GameplayEffectModifier>>();

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

        /// <summary>
        /// it's triggered only when the owner's gameplay effect is added or removed. 
        /// </summary>
        void RefreshModifierCache()
        {
            // 注销属性变化监听回调
            UnregisterAttributeChangedListen();
            _modifierCache.Clear();
            var gameplayEffects = _owner.GameplayEffectContainer.GameplayEffects();
            foreach (var geSpec in gameplayEffects)
            {
                if (geSpec.IsActive)
                {
                    foreach (var modifier in geSpec.Modifiers)
                    {
                        if (modifier.AttributeName == _processedAttribute.Name)
                        {
                            _modifierCache.Add(new Tuple<GameplayEffectSpec, GameplayEffectModifier>(geSpec, modifier));
                            TryRegisterAttributeChangedListen(geSpec, modifier);
                        }
                    }
                }
            }

            UpdateCurrentValueWhenModifierIsDirty();
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
                    foreach (var tuple in _modifierCache)
                    {
                        var spec = tuple.Item1;
                        var modifier = tuple.Item2;
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
                    foreach (var tuple in _modifierCache)
                    {
                        var spec = tuple.Item1;
                        var modifier = tuple.Item2;

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
                    foreach (var tuple in _modifierCache)
                    {
                        var spec = tuple.Item1;
                        var modifier = tuple.Item2;

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
            foreach (var tuple in _modifierCache)
                TryUnregisterAttributeChangedListen(tuple.Item1, tuple.Item2);
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
            foreach (var tuple in _modifierCache)
            {
                var ge = tuple.Item1;
                var modifier = tuple.Item2;
                if (modifier.MMC is AttributeBasedModCalculation mmc &&
                    mmc.captureType == AttributeBasedModCalculation.GEAttributeCaptureType.Track &&
                    attribute.Name == mmc.attributeName)
                {
                    if ((mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Target &&
                         attribute.Owner == ge.Owner) ||
                        (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Source &&
                         attribute.Owner == ge.Source))
                    {
                        UpdateCurrentValueWhenModifierIsDirty();
                        break;
                    }
                }
            }
        }
    }
}