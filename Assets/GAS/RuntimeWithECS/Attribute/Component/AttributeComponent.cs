using Unity.Entities;

namespace GAS.RuntimeWithECS.Attribute.Component
{
    public struct AttributeComponent : IComponentData
    {
        public float BaseValue;
        public float CurrentValue;
        public float MinValue;
        public float MaxValue;
    }
}