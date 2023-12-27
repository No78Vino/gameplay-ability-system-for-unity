using System;
using GAS.Runtime.Attribute.Value;

namespace GAS.Runtime.Attribute
{
    public class AttributeBase
    {
        public readonly string Name;
        public readonly string SetName;
        public readonly string ShortName;
        protected Action<AttributeBase, float, float> _onPostAttributeChange;
        protected Action<AttributeBase, float, float> _onPostGameplayEffectExecute;
        protected Action<AttributeBase, float> _onPreAttributeChange;
        protected Action<AttributeBase, float> _onPreGameplayEffectExecute;
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
            _onPreAttributeChange?.Invoke(this, value);

            var oldValue = CurrentValue;
            _value.SetCurrentValue(value);

            _onPostAttributeChange?.Invoke(this, oldValue, value);
        }

        public void SetBaseValue(float value)
        {
            _onPreGameplayEffectExecute?.Invoke(this, value);

            var oldValue = _value.BaseValue;
            _value.SetBaseValue(value);

            _onPostGameplayEffectExecute?.Invoke(this, oldValue, value);
        }

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
    }
}