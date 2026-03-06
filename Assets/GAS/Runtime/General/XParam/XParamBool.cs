using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamBool:XParam
    {
        [LabelText("值")]
        [ShowInInspector]
        [BeanField(nameof(SetValue), Comment = "值")]
        public bool Value { get; private set; }
        
        public void SetValue(bool value)
        {
            Value = value;
        }
        
        public XParamBool(bool value)
        {
            Value = value;
        }
        
        public XParamBool()
        {
            Value = false;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                Value = false;
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                Value = false;
                return;
            }

            Value = bool.TryParse(strData, out var result) && result;
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object> { Value.ToString() };
            return result;
        }
#endif
    }
}