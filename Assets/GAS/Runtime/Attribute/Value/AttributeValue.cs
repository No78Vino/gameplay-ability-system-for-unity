using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public enum CalculateMode
    {
        [LabelText(SdfIconType.Stack, Text = "叠加计算")]
        Stacking,

        [LabelText(SdfIconType.GraphDownArrow, Text = "取最小值")]
        MinValueOnly,

        [LabelText(SdfIconType.GraphUpArrow, Text = "取最大值")]
        MaxValueOnly,
    }

    public struct AttributeValue
    {
        public AttributeValue(float baseValue,
            CalculateMode calculateMode = CalculateMode.Stacking,
            SupportedOperation supportedOperation = SupportedOperation.All,
            float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            BaseValue = baseValue;
            SupportedOperation = supportedOperation;
            CurrentValue = baseValue;
            CalculateMode = calculateMode;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public CalculateMode CalculateMode { get; }
        public SupportedOperation SupportedOperation { get; }

        public float BaseValue { get; private set; }
        public float CurrentValue { get; private set; }

        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        /// <summary>
        /// ignore min and max value, set current value directly
        /// </summary>
        public void SetCurrentValue(float value)
        {
            CurrentValue = value;
        }

        public void SetBaseValue(float value)
        {
            BaseValue = value;
        }

        public void SetMinValue(float min)
        {
            MinValue = min;
        }

        public void SetMaxValue(float max)
        {
            MaxValue = max;
        }

        public void SetMinMaxValue(float min, float max)
        {
            MinValue = min;
            MaxValue = max;
        }

        public bool IsSupportOperation(GEOperation operation)
        {
            return SupportedOperation.HasFlag((SupportedOperation)(1 << (int)operation));
        }
    }
}