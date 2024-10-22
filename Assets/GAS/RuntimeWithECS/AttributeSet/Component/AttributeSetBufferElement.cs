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

    public static class AttributeSetBufferElementExtension
    {
        public static int IndexOfAttrSetCode(this DynamicBuffer<AttributeSetBufferElement> attrSets, int attrSetCode)
        {
            for (var i = 0; i < attrSets.Length; i++)
                if (attrSets[i].Code == attrSetCode)
                    return i;
            return -1;
        }
    }
}