using System.Collections.Generic;
using GAS.Runtime.Attribute;
using GAS.Runtime.Effects;

namespace GAS.Runtime.AttributeSet
{
    public abstract class AbstractAttributeSet
    {
        private Dictionary<string, AttributeBase> _attributes;
        
        protected abstract void PreAttributeBaseChange(AttributeBase attribute, float newValue);
        protected abstract void PostAttributeBaseChange(AttributeBase attribute, float oldValue, float newValue);
        protected abstract void PreAttributeChange(AttributeBase attribute, float newValue);
        protected abstract void PostAttributeChange(AttributeBase attribute, float oldValue, float newValue);
        protected abstract void PreGameplayEffectExecute(GameplayEffect gameplayEffect, AttributeBase attribute, float newValue);
        protected abstract void PostGameplayEffectExecute(GameplayEffect gameplayEffect, AttributeBase attribute, float oldValue,
            float newValue);

        public AbstractAttributeSet()
        {
            _attributes = new Dictionary<string, AttributeBase>();
        }

        public void AddAttribute(AttributeBase attribute)
        {
            _attributes.Add(attribute.Name, attribute);
        }
        
        public bool RemoveAttribute(string attributeName)
        {
            return _attributes.Remove(attributeName);
        }
        
        internal void ChangeAttributeBase(string attributeName, float value)
        {
            if (_attributes.TryGetValue(attributeName, out var attribute))
            {
                PreAttributeBaseChange(attribute, value);
                
                var oldValue = attribute.BaseValue;
                attribute.Value.SetBaseValue(value);
                
                PostAttributeBaseChange(attribute, oldValue,value);
            }
        }
        
        internal void ChangeAttribute(string attributeName, float value)
        {
            if (_attributes.TryGetValue(attributeName, out var attribute))
            {
                PreAttributeChange(attribute, value);
                
                var oldValue = attribute.CurrentValue;
                attribute.Value.SetCurrentValue(value);
                
                PostAttributeChange(attribute, oldValue,value);
            }
        }
        
        public void ChangeAttributeByGameplayEffect(GameplayEffect gameplayEffect, string attributeName, float value)
        {
            if (_attributes.TryGetValue(attributeName, out var attribute))
            {
                PreGameplayEffectExecute(gameplayEffect, attribute, value);

                var oldValue = attribute.CurrentValue;
                attribute.Value.SetCurrentValue(value);

                PostGameplayEffectExecute(gameplayEffect, attribute, oldValue, value);
            }
        }
        
        // public void ChangeAttributeBaseByGameplayEffect(GameplayEffect gameplayEffect, string attributeName, float value)
        // {
        //     if (_attributes.TryGetValue(attributeName, out var attribute))
        //     {
        //         PreGameplayEffectExecute(gameplayEffect, attribute, value);
        //
        //         var oldValue = attribute.BaseValue;
        //         attribute.Value.SetBaseValue(value);
        //
        //         PostGameplayEffectExecute(gameplayEffect, attribute, oldValue, value);
        //     }
        // }
    }
}