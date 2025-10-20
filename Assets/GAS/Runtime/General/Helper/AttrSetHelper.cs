namespace GAS.Runtime
{
    public static class AttrSetHelper
    {
        public static int GetAttrIndexByCode(this BEAttrSet self, int attrCode)
        {
            for (var i = 0; i < self.Attributes.Length; i++)
                if (self.Attributes[i].Code == attrCode)
                    return i;
            return -1;
        }
    }
}