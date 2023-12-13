using GAS.General;

namespace GAS.Runtime.Attribute.Value
{
    public struct AttributeValue
    {
        float _baseValue;
        float _currentValue;

        public AttributeValue(float baseValue)
        {
            _baseValue = baseValue;
            _currentValue = baseValue;
        }

        public float BaseValue => _baseValue;
        public float CurrentValue => _currentValue;

        public void SetCurrentValue(float value)
        {
            _currentValue = value;
        }
        
        public void SetBaseValue(float value)
        {
            _baseValue = value;
        }
    }
}