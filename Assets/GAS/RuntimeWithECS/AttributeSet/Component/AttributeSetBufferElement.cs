using System.Collections.Generic;
using GAS.RuntimeWithECS.Attribute.Component;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.AttributeSet.Component
{
    [InternalBufferCapacity(50)]
    public struct AttributeSetBufferElement : IBufferElementData
    {
        public int Code;
        public NativeArray<AttributeData> Attributes;
    }
}