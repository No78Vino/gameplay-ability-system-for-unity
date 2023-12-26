using System;
using GAS.Runtime.Attribute;

namespace GAS.Runtime.AttributeSet
{
    public abstract class AttributeSet
    {
        public abstract AttributeBase this[string key] { get; }
        public readonly string[] AttributeNames;
        
        protected Action<AttributeBase, float, float> _onPostGameplayEffectExecute;
        protected Action<AttributeBase, float, float> _onPostAttributeChange;
        protected Action<AttributeBase, float> _onPreGameplayEffectExecute;
        protected Action<AttributeBase, float> _onPreAttributeChange;

        public void RegisterPreGameplayEffectExecute(Action<AttributeBase, float> action)
        {
            _onPreGameplayEffectExecute += action;
        }

        public void RegisterPostGameplayEffectExecute(Action<AttributeBase, float, float> action)
        {
            _onPostGameplayEffectExecute += action;
        }

        public void RegisterPreAttributeChange(Action<AttributeBase, float> action)
        {
            _onPreAttributeChange += action;
        }

        public void RegisterPostAttributeChange(Action<AttributeBase, float, float> action)
        {
            _onPostAttributeChange += action;
        }

        public void UnRegisterPreGameplayEffectExecute(Action<AttributeBase, float> action)
        {
            _onPreGameplayEffectExecute -= action;
        }

        public void UnRegisterPostGameplayEffectExecute(Action<AttributeBase, float, float> action)
        {
            _onPostGameplayEffectExecute -= action;
        }

        public void UnRegisterPreAttributeChange(Action<AttributeBase, float> action)
        {
            _onPreAttributeChange -= action;
        }

        public void UnRegisterPostAttributeChange(Action<AttributeBase, float, float> action)
        {
            _onPostAttributeChange -= action;
        }

        public virtual void Dispose()
        {
            _onPreGameplayEffectExecute = null;
            _onPostGameplayEffectExecute = null;
            _onPreAttributeChange = null;
            _onPostAttributeChange = null;
        }

        public void ChangeAttributeBase(AttributeBase attribute, float value)
        {
            _onPreGameplayEffectExecute?.Invoke(attribute, value);

            var oldValue = attribute.BaseValue;
            attribute.SetBaseValue(value);

            _onPostGameplayEffectExecute?.Invoke(attribute, oldValue, value);
        }

        public void ChangeAttribute(AttributeBase attribute, float value)
        {
            _onPreAttributeChange?.Invoke(attribute, value);

            var oldValue = attribute.CurrentValue;
            attribute.SetCurrentValue(value);

            _onPostAttributeChange?.Invoke(attribute, oldValue, value);
        }
    }
}