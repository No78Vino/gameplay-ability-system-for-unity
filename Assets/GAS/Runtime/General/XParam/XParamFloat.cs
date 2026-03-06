using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamFloat:XParam
    {
        [LabelText("值")]
        [ShowInInspector]
        [BeanField(nameof(SetValue), Comment = "值")]
        public float Value { get; private set; }
        
        public XParamFloat()
        {
            Value = 0;
        }
        
        public XParamFloat(float v)
        {
            Value = v;
        }

        public void SetValue(float v)
        {
            Value = v;
        }

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            Value = paramData.Count > 0 ? float.Parse(paramData[0].ToString()) : 0;
        }

        public List<object> EncodeExcelData()
        {
            var data = new List<object> { Value };
            return data;
        }
#endif
    }
}