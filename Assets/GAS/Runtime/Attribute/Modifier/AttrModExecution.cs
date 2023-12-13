using GAS.Runtime.Attribute.Value;

namespace GAS.Runtime.Attribute.Modifier
{
    public class AttrModExecution
    {
        public float AddBase;
        public float AddAbsolute;
        public float MultiplyBase;
        public float MultiplyBaseWithAdditive;
        public float Override;
        
        public bool IsOverride;
        
        public static AttrModExecution operator +(AttrModExecution a, AttrModExecution b)
        {
            a.AddBase += b.AddBase;
            a.AddAbsolute += b.AddAbsolute;
            a.MultiplyBase += b.MultiplyBase;
            a.MultiplyBaseWithAdditive += b.MultiplyBaseWithAdditive;
        
            if (a.IsOverride || b.IsOverride)
            {
                a.IsOverride = true;
                if (b.IsOverride) a.Override = b.Override;
            }
        
            return a;
        }
        
        public static AttrModExecution operator +(AttrModExecution a, AttributeBase b)
        {
            switch (b.Modifier.Type)
            {
                case AttributeModifierType.Override:
                    a.IsOverride = true;
                    a.Override = b.CurrentValue;
                    break;
                case AttributeModifierType.AddAbsolute:
                    a.AddAbsolute += b.CurrentValue;
                    break;
                case AttributeModifierType.AddBase:
                    a.AddBase += b.CurrentValue;
                    break;
                case AttributeModifierType.MultiplyBase:
                    a.MultiplyBase += b.CurrentValue;
                    break;
                case AttributeModifierType.MultiplyBaseWithAdditive:
                    a.MultiplyBaseWithAdditive += b.CurrentValue;
                    break;
            }
            return a;
        }
        
        public static AttributeValue operator +(AttributeValue a, AttrModExecution b)
        {
            float currentValue;
            if (b.IsOverride)
                currentValue = b.Override;
            else
                currentValue =
                    (a.BaseValue * (1 + b.MultiplyBase) + b.AddBase) * (1 + b.MultiplyBaseWithAdditive) + b.AddAbsolute;
        
            a.SetCurrentValue(currentValue);
            return a;
        }
    }
}