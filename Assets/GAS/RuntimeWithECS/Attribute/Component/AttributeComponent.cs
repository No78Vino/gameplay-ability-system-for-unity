using Unity.Entities;

namespace GAS.RuntimeWithECS.Attribute.Component
{
    public struct AttributeComponent : IComponentData
    {
        public int Code;
        public float BaseValue;
        public float CurrentValue;
        public float MinValue;
        public float MaxValue;

        public static readonly AttributeComponent NULL = new()
        {
            Code = -1
        };
    }
}