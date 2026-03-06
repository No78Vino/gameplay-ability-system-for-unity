using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamInt: XParam
    {
        [LabelText("值")]
        [ShowInInspector]
        [BeanField(nameof(SetValue), Comment = "值")]
        public int Value { get; private set; }
        
        public void SetValue(int value)
        {
            Value = value;
        }
        
        public XParamInt(int value)
        {
            Value = value;
        }
        
        public XParamInt()
        {
            Value = 0;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                Value = 0;
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                Value = 0;
                return;
            }

            Value = !int.TryParse(strData, out var result) ? 0 : result;
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object> { Value.ToString() };
            return result;
        }
#endif
    }
}