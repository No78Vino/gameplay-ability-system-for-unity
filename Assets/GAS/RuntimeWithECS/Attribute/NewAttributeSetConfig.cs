namespace GAS.RuntimeWithECS.Attribute
{
    public struct NewAttributeSetConfig
    {
        public readonly int Code;
        public readonly AttributeBaseSetting[] Settings;

        public NewAttributeSetConfig(int code,AttributeBaseSetting[] settings)
        {
            Code = code;
            Settings = settings;
        }
    }

    public struct AttributeBaseSetting
    {
        public readonly int Code;
        public readonly float InitValue;
        public readonly float Min;
        public readonly float Max;
        
        public AttributeBaseSetting(int code, float initValue,float min,float max)
        {
            Code = code;
            InitValue = initValue;
            Min = min;
            Max = max;
        }
    }
}