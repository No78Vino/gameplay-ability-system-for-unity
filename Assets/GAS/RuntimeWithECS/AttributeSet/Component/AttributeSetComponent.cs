using GAS.RuntimeWithECS.Attribute.Component;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.AttributeSet.Component
{
    public struct AttributeSetComponent : IBufferElementData
    {
        public int Code;
        public NativeArray<AttributeData> Attributes;
    }
}