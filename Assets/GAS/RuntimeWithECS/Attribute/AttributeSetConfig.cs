namespace GAS.RuntimeWithECS.Attribute
{
    public struct AttributeSetConfig
    {
        public readonly int CodeValue;
        public readonly int[] Attributes;

        public AttributeSetConfig(int codeValue, int[] attributes)
        {
            CodeValue = codeValue;
            Attributes = attributes;
        }
    }
}