using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class ParamString:IExParameterBase
    {
        [LabelText("文本值")]
        [ShowInInspector]
        public string Value { get; private set; }
        
        public ParamString()
        {
            Value = string.Empty;
        }
        
        public ParamString(string v)
        {
            Value = v;
        }

        public void SetValue(string v)
        {
            Value = v;
        }

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            Value = paramData.Count > 0 ? paramData[0].ToString() : string.Empty;
        }

        public List<object> EncodeExcelData()
        {
            var data = new List<object> { Value };
            return data;
        }
#endif
    }
}