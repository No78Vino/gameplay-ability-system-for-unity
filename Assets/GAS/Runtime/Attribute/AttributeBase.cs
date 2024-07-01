using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        protected IEnumerable<Func<AttributeBase, float, float>> _preBaseValueChangeListeners;

        private AttributeValue _value;
        private AbilitySystemComponent _owner;
        public AbilitySystemComponent Owner => _owner;

        public AttributeBase(string attrSetName, string attrName, float value = 0,
            CalculateMode calculateMode = CalculateMode.Stacking,
            SupportedOperation supportedOperation = SupportedOperation.All,
            float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            SetName = attrSetName;
            Name = $"{attrSetName}.{attrName}";
            ShortName = attrName;
            _value = new AttributeValue(value, calculateMode, supportedOperation, minValue, maxValue);
        }


        public AttributeValue Value => _value;
        public float BaseValue => _value.BaseValue;
        public float CurrentValue => _value.CurrentValue;

        public float MinValue => _value.MinValue;
        public float MaxValue => _value.MaxValue;

        public CalculateMode CalculateMode => _value.CalculateMode;
        public SupportedOperation SupportedOperation => _value.SupportedOperation;

        public void SetOwner(AbilitySystemComponent owner)
        {
            _owner = owner;
        }

        public void SetMinValue(float min)
        {
            _value.SetMinValue(min);
        }

        public void SetMaxValue(float max)
        {
            _value.SetMaxValue(max);
        }

        public void SetMinMaxValue(float min, float max)
        {
            _value.SetMinValue(min);
            _value.SetMaxValue(max);
        }
        
        public bool IsSupportOperation(GEOperation operation)
        {
            return _value.IsSupportOperation(operation);
        }

        public void SetCurrentValue(float value)
        {
            value = Mathf.Clamp(value, _value.MinValue, _value.MaxValue);

            _onPreCurrentValueChange?.Invoke(this, value);

            var oldValue = CurrentValue;
            _value.SetCurrentValue(value);

            if (!Mathf.Approximately(oldValue, value)) _onPostCurrentValueChange?.Invoke(this, oldValue, value);
        }

        public void SetBaseValue(float value)
        {
            if (_onPreBaseValueChange != null)
            {
                value = InvokePreBaseValueChangeListeners(value);
            }

            var oldValue = _value.BaseValue;
            _value.SetBaseValue(value);

            if (!Mathf.Approximately(oldValue, value)) _onPostBaseValueChange?.Invoke(this, oldValue, value);
        }

        public void SetCurrentValueWithoutEvent(float value)
        {
            _value.SetCurrentValue(value);
        }

        public void SetBaseValueWithoutEvent(float value)
        {
            _value.SetBaseValue(value);
        }

        public void RegisterPreBaseValueChange(Func<AttributeBase, float, float> func)
        {
            _onPreBaseValueChange += func;
            _preBaseValueChangeListeners =
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

        public void UnregisterPreBaseValueChange(Func<AttributeBase, float, float> func)
        {
            _onPreBaseValueChange -= func;
            _preBaseValueChangeListeners =
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
            if (_preBaseValueChangeListeners == null) return value;

            foreach (var t in _preBaseValueChangeListeners)
                value = t.Invoke(this, value);
            return value;
        }
    }
}