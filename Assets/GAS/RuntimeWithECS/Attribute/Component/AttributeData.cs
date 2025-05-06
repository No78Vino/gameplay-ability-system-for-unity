using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct AttributeData : IComponentData
    {
        public int Code;
        public float BaseValue;
        public float CurrentValue;
        public bool IsClampMin;
        public bool IsClampMax;
        public float MinValue;
        public float MaxValue;
        public bool Dirty;
        
        public static readonly AttributeData NULL = new()
        {
            Code = -1
        };
    }

    public static class AttributeDataExtension
    {
        public static int IndexOfAttrCode(this NativeArray<AttributeData> attrs, int attrCode)
        {
            for (var i = 0; i < attrs.Length; i++)
                if (attrs[i].Code == attrCode)
                    return i;
            return -1;
        }
    }
}