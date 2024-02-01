using System;
using System.Collections.Generic;
using GAS.Runtime.Component;
using GAS.Runtime.Effects;
using GAS.Runtime.Effects.Modifier;

namespace GAS.Runtime.Attribute
{
    public class AttributeAggregator
    {
        AttributeBase _processedAttribute;
        AbilitySystemComponent _owner;

        /// <summary>
        ///  the order of the modifiers is important.
        /// </summary>
        private List<Tuple<GameplayEffectSpec, GameplayEffectModifier>> _modifierCahce =
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
        }
        
        void OnDispose()
        {
            _processedAttribute.UnregisterPostBaseValueChange(UpdateCurrentValueWhenBaseValueIsDirty);
            _owner.GameplayEffectContainer.UnregisterOnGameplayEffectContainerIsDirty(RefreshModifierCache);
        }
        
        /// <summary>
        /// it's triggered only when the owner's gameplay effect is added or removed. 
        /// </summary>
        void RefreshModifierCache()
        {
            _modifierCahce.Clear();
            var activeGameplayEffects = _owner.GameplayEffectContainer.GetActiveGameplayEffects();
            foreach (var geSpec in activeGameplayEffects)
            {
                foreach (var modifier in geSpec.GameplayEffect.Modifiers)
                {
                    if (modifier.AttributeName == _processedAttribute.Name)
                    {
                        _modifierCahce.Add(new Tuple<GameplayEffectSpec, GameplayEffectModifier>(geSpec,modifier));
                    }
                }
            }
            
            UpdateCurrentValueWhenModifierIsDirty();
        }
        
        /// <summary>
        /// Calculate the new Value for the CurrentValue.
        /// (BaseValue's changes depend on the instant GameplayEffect.)
        /// this method is triggered when the _modifierCahce is changed or the _processedAttribute's BaseValue is changed.
        /// </summary>
        /// <returns></returns>
        float CalculateNewValue()
        {
            float newValue = _processedAttribute.BaseValue;
            foreach (var tuple in _modifierCahce)
            {
                var spec = tuple.Item1;
                var modifier = tuple.Item2;
                var magnitude = modifier.MMC.CalculateMagnitude(spec,modifier.ModiferMagnitude);
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
    }
}