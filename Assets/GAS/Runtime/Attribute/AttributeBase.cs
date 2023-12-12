namespace GAS.Runtime.Attribute
{
    public class AttributeBase
    {
        float _baseValue;
        float _currentValue;

        public AttributeBase(float baseValue)
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
    }
}