using System;
using GAS.Runtime.Attribute.Value;

namespace GAS.Runtime.Attribute
{
    public class AttributeBase
    {
        public readonly string Name;
        public readonly string SetName;
        public readonly string ShortName;
        protected event Action<AttributeBase, float, float> _onPostCurrentValueChange;
        protected event Action<AttributeBase, float, float> _onPostBaseValueChange;
        protected event Func<AttributeBase, float, float> _onPreCurrentValueChange;
        protected event Func<AttributeBase, float, float> _onPreBaseValueChange;
        
        private AttributeValue _value;

        public AttributeBase(string attrSetName, string attrName, float value)
        {
            SetName = attrSetName;
            Name = $"{attrSetName}.{attrName}";
            ShortName = attrName;
            _value = new AttributeValue(value);
        }

        public AttributeBase(string attrSetName, string attrName)
        {
            SetName = attrSetName;
            Name = $"{attrSetName}.{attrName}";
            ShortName = attrName;
            _value = new AttributeValue(0);
        }

        public AttributeValue Value => _value;
        public float BaseValue => _value.BaseValue;
        public float CurrentValue => _value.CurrentValue;

        public void SetCurrentValue(float value)
        {
            if (_onPreCurrentValueChange != null)
            {
                value = _onPreCurrentValueChange.Invoke(this, value);
            }

            var oldValue = CurrentValue;
            _value.SetCurrentValue(value);

            _onPostCurrentValueChange?.Invoke(this, oldValue, value);
        }

        public void SetBaseValue(float value)
        {
            if (_onPreBaseValueChange != null)
            {
                value = _onPreBaseValueChange.Invoke(this, value);
            }

            var oldValue = _value.BaseValue;
            _value.SetBaseValue(value);

            _onPostBaseValueChange?.Invoke(this, oldValue, value);
        }
        
        public void SetCurrentValueWithoutEvent(float value)
        {
            _value.SetCurrentValue(value);
        }
        
        public void SetBaseValueWithoutEvent(float value)
        {
            _value.SetBaseValue(value);
        }

        public void RegisterPreBaseValueChange(Func<AttributeBase, float,float> func)
        {
            _onPreBaseValueChange += func;
        }

        public void RegisterPostBaseValueChange(Action<AttributeBase, float, float> action)
        {
            _onPostBaseValueChange += action;
        }

        public void RegisterPreCurrentValueChange(Func<AttributeBase, float,float> func)
        {
            _onPreCurrentValueChange += func;
        }

        public void RegisterPostCurrentValueChange(Action<AttributeBase, float, float> action)
        {
            _onPostCurrentValueChange += action;
        }

        public void UnregisterPreBaseValueChange(Func<AttributeBase, float,float> func)
        {
            _onPreBaseValueChange -= func;
        }

        public void UnregisterPostBaseValueChange(Action<AttributeBase, float, float> action)
        {
            _onPostBaseValueChange -= action;
        }

        public void UnregisterPreCurrentValueChange(Func<AttributeBase, float,float> func)
        {
            _onPreCurrentValueChange -= func;
        }

        public void UnregisterPostCurrentValueChange(Action<AttributeBase, float, float> action)
        {
            _onPostCurrentValueChange -= action;
        }

        public virtual void Dispose()
        {
            _onPreBaseValueChange = null;
            _onPostBaseValueChange = null;
            _onPreCurrentValueChange = null;
            _onPostCurrentValueChange = null;
        }
    }
}