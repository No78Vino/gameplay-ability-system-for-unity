using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class CueParamString:ICueParameter
    {
        [LabelText("文本值")]
        [ShowInInspector]
        public string Value { get; private set; }
        
        public CueParamString()
        {
            Value = string.Empty;
        }
        
        public CueParamString(string v)
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