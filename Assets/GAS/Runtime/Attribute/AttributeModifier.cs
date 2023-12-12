namespace GAS.Runtime.Attribute
{
    public struct AttributeModifier
    {
        public float AddBase;
        public float AddAbsolute;
        public float MultiplyBase;
        public float MultiplyBaseWithAdditive;

        public bool isOverride;
        public float Override;
        
        public static AttributeModifier operator +(AttributeModifier a, AttributeModifier b)
        {
            a.AddBase += b.AddBase;
            a.AddAbsolute += b.AddAbsolute;
            a.MultiplyBase += b.MultiplyBase;
            a.MultiplyBaseWithAdditive += b.MultiplyBaseWithAdditive;

            if (a.isOverride || b.isOverride)
            {
                a.isOverride = true;
                if (b.isOverride)
                {
                    a.Override = b.Override;
                }
            }
            return a;
        }

        public static AttributeBase operator +(AttributeBase a, AttributeModifier b)
        {
            float currentValue;
            if (b.isOverride)
            {
                currentValue = b.Override;
            }
            else
            {
                currentValue =
                    (a.BaseValue * b.MultiplyBase + b.AddBase) * b.MultiplyBaseWithAdditive + b.AddAbsolute;
            }

            a.SetCurrentValue(currentValue);
            return a;
        }
    }
}