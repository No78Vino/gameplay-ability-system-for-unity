using GAS.RuntimeWithECS.Attribute.Component;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.AttributeSet.Component
{
    public struct AttributeSetComponent : IComponentData
    {
        public int Code;
        public NativeArray<int> attributeCodes;
        public NativeArray<AttributeComponent> attributes;
    }
}