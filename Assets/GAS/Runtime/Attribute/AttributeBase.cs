using GAS.Runtime.Attribute.Modifier;
using GAS.Runtime.Attribute.Value;

namespace GAS.Runtime.Attribute
{
    public class AttributeBase
    {
        public string Name { get; private set; }
        private AttributeModifier _modifier;
        private AttributeValue _value;

        public AttributeBase(string name,float value, 
            AttributeModifierType type = AttributeModifierType.None,
            AttributeCombineTiming timing = AttributeCombineTiming.Sequence,
            AttributeCombineType combineType = AttributeCombineType.Single)
        {
            Name = name;
            _modifier = new AttributeModifier(type, timing, combineType);
            _value = new AttributeValue(value);
        }

        public AttributeValue Value => _value;
        public float BaseValue => _value.BaseValue;
        public float CurrentValue => _value.CurrentValue;
        public float AdditionValue => _value.CurrentValue - _value.BaseValue;
        public AttributeModifier Modifier => _modifier;
        
        /// <summary>
        /// Get the copy of this attribute
        /// </summary>
        /// <returns></returns>
        public virtual AttributeBase DeepCopy()
        {
            var copy = new AttributeBase(Name,_value.BaseValue, _modifier.Type, _modifier.Timing, _modifier.CombineType);
            copy._value.SetCurrentValue(_value.CurrentValue);
            return copy;
        }
        
        public virtual AttributeBase ShallowCopy()
        {
            return this;
        }
    }
}