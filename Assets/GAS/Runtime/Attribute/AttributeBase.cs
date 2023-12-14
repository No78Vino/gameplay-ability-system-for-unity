using GAS.Runtime.Attribute.Value;

namespace GAS.Runtime.Attribute
{
    public class AttributeBase
    {
        public string Name { get; private set; }
        private AttributeValue _value;

        public AttributeBase(string name,float value)
        {
            Name = name;
            _value = new AttributeValue(value);
        }

        public AttributeValue Value => _value;
        public float BaseValue => _value.BaseValue;
        public float CurrentValue => _value.CurrentValue;
        public float AdditionValue => _value.CurrentValue - _value.BaseValue;
        
        /// <summary>
        /// Get the copy of this attribute
        /// </summary>
        /// <returns></returns>
        public virtual AttributeBase DeepCopy()
        {
            var copy = new AttributeBase(Name,_value.BaseValue);
            copy._value.SetCurrentValue(_value.CurrentValue);
            return copy;
        }
        
        public virtual AttributeBase ShallowCopy()
        {
            return this;
        }
    }
}