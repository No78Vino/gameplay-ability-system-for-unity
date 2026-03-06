using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamString : XParam
    {
        [LabelText("值")]
        [ShowInInspector]
        [BeanField(nameof(SetValue), Comment = "值")]
        public string Value { get; private set; }
        
        public XParamString()
        {
            Value = string.Empty;
        }
        
        public XParamString(string value)
        {
            SetValue(value);
        }

        public void SetValue(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                Value = string.Empty;
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                Value = string.Empty;
                return;
            }

            Value = strData;
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object>();
            if (string.IsNullOrEmpty(Value))
            {
                result.Add(string.Empty);
                return result;
            }

            result.Add(Value);
            return result;
        }
#endif
    }
}