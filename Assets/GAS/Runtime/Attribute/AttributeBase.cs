using System;
using System.Collections.Generic;
using System.Linq;

namespace GAS.Runtime
{
    public class AttributeBase
    {
        public readonly string Name;
        public readonly string SetName;
        public readonly string ShortName;
        protected event Action<AttributeBase, float, float> _onPostCurrentValueChange;
        protected event Action<AttributeBase, float, float> _onPostBaseValueChange;
        protected event Action<AttributeBase, float> _onPreCurrentValueChange;
        protected event Func<AttributeBase, float, float> _onPreBaseValueChange;
        protected IEnumerable<Func<AttributeBase, float, float>> _preBaseValueChangelisteners;
        
        private AttributeValue _value;
        private AbilitySystemComponent _owner;
        public AbilitySystemComponent Owner => _owner;

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

        public void SetOwner(AbilitySystemComponent owner)
        {
            _owner = owner;
        }
        
        public void SetCurrentValue(float value)
        {
            _onPreCurrentValueChange?.Invoke(this, value);

            var oldValue = CurrentValue;
            _value.SetCurrentValue(value);

            if (oldValue != value) _onPostCurrentValueChange?.Invoke(this, oldValue, value);
        }

        public void SetBaseValue(float value)
        {
            if (_onPreBaseValueChange != null)
            {
                value = InvokePreBaseValueChangeListeners(value);
            }
            
            var oldValue = _value.BaseValue;
            _value.SetBaseValue(value);

            if (oldValue != value) _onPostBaseValueChange?.Invoke(this, oldValue, value);
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
            _preBaseValueChangelisteners =
                _onPreBaseValueChange?.GetInvocationList().Cast<Func<AttributeBase, float, float>>();
        }

        public void RegisterPostBaseValueChange(Action<AttributeBase, float, float> action)
        {
            _onPostBaseValueChange += action;
        }

        public void RegisterPreCurrentValueChange(Action<AttributeBase, float> action)
        {
            _onPreCurrentValueChange += action;
        }

        public void RegisterPostCurrentValueChange(Action<AttributeBase, float, float> action)
        {
            _onPostCurrentValueChange += action;
        }

        public void UnregisterPreBaseValueChange(Func<AttributeBase, float,float> func)
        {
            _onPreBaseValueChange -= func;
            _preBaseValueChangelisteners =
                _onPreBaseValueChange?.GetInvocationList().Cast<Func<AttributeBase, float, float>>();
        }

        public void UnregisterPostBaseValueChange(Action<AttributeBase, float, float> action)
        {
            _onPostBaseValueChange -= action;
        }

        public void UnregisterPreCurrentValueChange(Action<AttributeBase, float> action)
        {
            _onPreCurrentValueChange -= action;
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
        
        private float InvokePreBaseValueChangeListeners(float value)
        {
            if (_preBaseValueChangelisteners == null) return value;
            
            foreach (var t in _preBaseValueChangelisteners)
                value = t.Invoke(this, value);
            return value;
        }
    }
}