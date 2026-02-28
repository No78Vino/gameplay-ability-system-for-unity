using System;  
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class AttributeBasedMmcParam : XParam
    {
        [ShowInInspector]
        [LabelText("属性集Code")]
        public int AttrSetCode { get; private set; }

        [ShowInInspector]
        [LabelText("属性Code")]
        public int AttrCode { get; private set; }

        [ShowInInspector] 
        [LabelText("来源类型")]
        public AttributeFromType FromType { get; private set; }

        [ShowInInspector]
        [LabelText("属性捕获类型")]
        public AttributeCaptureType CaptureType { get; private set; }

        [ShowInInspector] [LabelText("系数K")] public float K { get; private set; } = 1f;

        [ShowInInspector] [LabelText("偏移B")] public float B { get; private set; } = 0f;

        public void SetAttrSetCode(int v) => AttrSetCode = v;
        public void SetAttrCode(int v) => AttrCode = v;
        public void SetFromType(int v) => FromType = (AttributeFromType)v;
        public void SetCaptureType(int v) => CaptureType = (AttributeCaptureType)v;
        public void SetK(float v) => K = v;
        public void SetB(float v) => B = v;

        private static IAttributeValueResolver _resolver;

        public static IAttributeValueResolver GetResolver() => _resolver ??= new DefaultAttributeValueResolver();

#if UNITY_EDITOR
        public List<object> EncodeExcelData() => new List<object>
            { AttrSetCode, AttrCode, (int)FromType, (int)CaptureType, K, B };

        public void DecodeExcelData(List<object> data)
        {
            if (data == null || data.Count < 6) return;
            AttrSetCode = Convert.ToInt32(data[0]);
            AttrCode = Convert.ToInt32(data[1]);
            FromType = (AttributeFromType)Convert.ToInt32(data[2]);
            CaptureType = (AttributeCaptureType)Convert.ToInt32(data[3]);
            K = Convert.ToSingle(data[4]);
            B = Convert.ToSingle(data[5]);
        }
#endif
    }
}