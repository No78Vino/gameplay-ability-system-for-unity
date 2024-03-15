namespace GAS.Runtime
{
    public struct AttributeValue
    {
        public AttributeValue(float baseValue)
        {
            _baseValue = baseValue;
            _currentValue = baseValue;
        }

        float _baseValue;
        public float BaseValue => _baseValue;

        float _currentValue;
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