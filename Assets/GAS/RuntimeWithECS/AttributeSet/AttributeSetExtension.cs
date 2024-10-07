using GAS.RuntimeWithECS.AttributeSet.Component;

namespace GAS.RuntimeWithECS.AttributeSet
{
    public static class AttributeSetExtension
    {
        public static int GetAttrIndexByCode(this AttributeSetBufferElement self, int attrCode)
        {
            for (var i = 0; i < self.Attributes.Length; i++)
                if (self.Attributes[i].Code == attrCode)
                    return i;
            return -1;
        }
    }
}