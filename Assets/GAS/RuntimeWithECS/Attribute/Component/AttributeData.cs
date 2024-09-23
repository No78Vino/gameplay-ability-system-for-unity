using Unity.Entities;

namespace GAS.RuntimeWithECS.Attribute.Component
{
    public struct AttributeData
    {
        public int Code;
        public float BaseValue;
        public float CurrentValue;
        public float MinValue;
        public float MaxValue;
        
        public static readonly AttributeData NULL = new()
        {
            Code = -1
        };
    }
}