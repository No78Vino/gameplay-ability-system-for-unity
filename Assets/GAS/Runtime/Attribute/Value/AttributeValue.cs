namespace GAS.Runtime.Attribute.Value
{
    public struct AttributeValue
    {
        public AttributeValue(float baseValue)
        {
            BaseValue = baseValue;
            CurrentValue = baseValue;
        }

        public float BaseValue { get; private set; }

        public float CurrentValue { get; private set; }

        public void SetCurrentValue(float value)
        {
            CurrentValue = value;
        }

        public void SetBaseValue(float value)
        {
            BaseValue = value;
        }
    }
}