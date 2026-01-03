using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [InternalBufferCapacity(50)]
    public struct BEAttrSet : IBufferElementData
    {
        public int Code;
        public NativeArray<CAttributeData> Attributes;
    }

    [BurstCompile]
    public static class AttributeSetBufferElementExtension
    {
        public static int IndexOfAttrSetCode(this DynamicBuffer<BEAttrSet> attrSets, int attrSetCode)
        {
            for (var i = 0; i < attrSets.Length; i++)
                if (attrSets[i].Code == attrSetCode)
                    return i;
            return -1;
        }
    }
}