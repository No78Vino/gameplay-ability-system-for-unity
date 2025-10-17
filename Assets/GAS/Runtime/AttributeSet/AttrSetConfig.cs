namespace GAS.Runtime
{
    public struct AttrSetConfig
    {
        public readonly int Code;
        public readonly AttributeBaseSetting[] Settings;

        public AttrSetConfig(int code,AttributeBaseSetting[] settings)
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
        public readonly bool IsClampMin;
        public readonly bool IsClampMax;
        
        public AttributeBaseSetting(int code, float initValue,bool isClampMin,bool isClampMax,float min,float max)
        {
            Code = code;
            InitValue = initValue;
            Min = min;
            Max = max;
            IsClampMin = isClampMin;
            IsClampMax = isClampMax;
        }
    }
}