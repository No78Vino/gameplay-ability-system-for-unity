using System;
using System.Collections.Generic;

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
        
        public AttributeAggregator(AttributeBase attribute , AbilitySystemComponent owner)
        {
            _processedAttribute = attribute;
            _owner = owner;

            OnCreated();
        }

        void OnCreated()
        {
            _processedAttribute.RegisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueIsDirty);
            _owner.GameplayEffectContainer.RegisterOnGameplayEffectContainerIsDirty(RefreshModifierCache);
            GASEvents.AttributeChanged.Subscribe(OnAttributeChanged);
        }
        
        void OnDispose()
        {
            _processedAttribute.UnregisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueIsDirty);
            _owner.GameplayEffectContainer.UnregisterOnGameplayEffectContainerIsDirty(RefreshModifierCache);
            GASEvents.AttributeChanged.Unsubscribe(OnAttributeChanged);
        }
        
        /// <summary>
        /// it's triggered only when the owner's gameplay effect is added or removed. 
        /// </summary>
        void RefreshModifierCache()
        {
            _modifierCache.Clear();
            var gameplayEffects = _owner.GameplayEffectContainer.GameplayEffects();
            foreach (var geSpec in gameplayEffects)
            {
                if (geSpec.IsActive)
                {
                    foreach (var modifier in geSpec.GameplayEffect.Modifiers)
                    {
                        if (modifier.AttributeName == _processedAttribute.Name)
                        {
                            _modifierCache.Add(new Tuple<GameplayEffectSpec, GameplayEffectModifier>(geSpec, modifier));
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
            float newValue = _processedAttribute.BaseValue;
            foreach (var tuple in _modifierCache)
            {
                var spec = tuple.Item1;
                var modifier = tuple.Item2;
                var magnitude = modifier.CalculateMagnitude(spec,modifier.ModiferMagnitude);
                switch (modifier.Operation)
                {
                    case GEOperation.Add:
                        newValue += magnitude;
                        break;
                    case GEOperation.Multiply:
                        newValue *= magnitude;
                        break;
                    case GEOperation.Override:
                        newValue = magnitude;
                        break;
                }
            }
            return newValue;
        }
        
        void UpdateCurrentValueWhenBaseValueIsDirty(AttributeBase attribute, float oldBaseValue, float newBaseValue)
        {
            if(oldBaseValue == newBaseValue) return;
            
            float newValue = CalculateNewValue();
            _processedAttribute.SetCurrentValue(newValue);
        }
        
        void UpdateCurrentValueWhenModifierIsDirty()
        {
            float newValue = CalculateNewValue();
            _processedAttribute.SetCurrentValue(newValue);
        }

        private void OnAttributeChanged(object sender, AttributeChangedEventArgs e)
        {
            if(_modifierCache.Count == 0) return;
            foreach (var tuple in _modifierCache)
            {
                var ge = tuple.Item1;
                var modifier = tuple.Item2;
                if (modifier.MMC is AttributeBasedModCalculation mmc &&
                    mmc.captureType == AttributeBasedModCalculation.GEAttributeCaptureType.Track &&
                    e.Attribute.Name == modifier.AttributeName)
                {
                    if ((mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Target &&
                         e.Owner == ge.Owner) ||
                        (mmc.attributeFromType == AttributeBasedModCalculation.AttributeFrom.Source &&
                         e.Owner == ge.Source))
                    {
                        UpdateCurrentValueWhenModifierIsDirty();
                        break;
                    }
                }
            }
        }
    }
}