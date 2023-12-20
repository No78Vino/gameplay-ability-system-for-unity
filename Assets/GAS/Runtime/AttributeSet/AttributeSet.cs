using System;
using GAS.Runtime.Attribute;

namespace GAS.Runtime.AttributeSet
{
    public abstract class AttributeSet
    {
        public abstract AttributeBase this[string key] { get; }
        
        protected Action<AttributeBase, float, float> _onPostAttributeBaseChange;
        protected Action<AttributeBase, float, float> _onPostAttributeChange;
        protected Action<AttributeBase, float> _onPreAttributeBaseChange;
        protected Action<AttributeBase, float> _onPreAttributeChange;

        public void RegisterPreAttributeBaseChange(Action<AttributeBase, float> action)
        {
            _onPreAttributeBaseChange += action;
        }

        public void RegisterPostAttributeBaseChange(Action<AttributeBase, float, float> action)
        {
            _onPostAttributeBaseChange += action;
        }

        public void RegisterPreAttributeChange(Action<AttributeBase, float> action)
        {
            _onPreAttributeChange += action;
        }

        public void RegisterPostAttributeChange(Action<AttributeBase, float, float> action)
        {
            _onPostAttributeChange += action;
        }

        public void UnRegisterPreAttributeBaseChange(Action<AttributeBase, float> action)
        {
            _onPreAttributeBaseChange -= action;
        }

        public void UnRegisterPostAttributeBaseChange(Action<AttributeBase, float, float> action)
        {
            _onPostAttributeBaseChange -= action;
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
            _onPreAttributeBaseChange = null;
            _onPostAttributeBaseChange = null;
            _onPreAttributeChange = null;
            _onPostAttributeChange = null;
        }

        public void ChangeAttributeBase(AttributeBase attribute, float value)
        {
            _onPreAttributeBaseChange?.Invoke(attribute, value);

            var oldValue = attribute.BaseValue;
            attribute.Value.SetBaseValue(value);

            _onPostAttributeBaseChange?.Invoke(attribute, oldValue, value);
        }

        public void ChangeAttribute(AttributeBase attribute, float value)
        {
            _onPreAttributeChange?.Invoke(attribute, value);

            var oldValue = attribute.CurrentValue;
            attribute.Value.SetCurrentValue(value);

            _onPostAttributeChange?.Invoke(attribute, oldValue, value);
        }
        
        
    }
}