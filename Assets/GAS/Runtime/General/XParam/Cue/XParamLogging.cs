using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamLogging:XParam
    {
        [LabelText("日志")]
        [ShowInInspector]
        [BeanField(nameof(SetValue), Comment = "日志",Order = 1)]
        public string Value { get; private set; }
        
        [LabelText("持续时长")]
        [ShowInInspector]
        [BeanField(nameof(SetDuration), Comment = "持续时长",Order = 2)]
        public float Duration;
        
        public XParamLogging()
        {
            Value = string.Empty;
        }
        
        public XParamLogging(string value,float duration)
        {
            SetValue(value);
            SetDuration(duration);
        }

        public void SetValue(string value)
        {
            Value = value;
        }
        
        public void SetDuration(float duration)
        {
            Duration = duration;
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

            Value = strData == XParamDefault.DefaultString ? string.Empty : strData;
            
            var durationData = paramData.Count > 1 ? paramData[1] : null;
            if (durationData != null)
                 float.TryParse(durationData as string,out Duration);

        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object>();
            if (string.IsNullOrEmpty(Value))
            {
                result.Add(XParamDefault.DefaultString);
                result.Add(0f);
                return result;
            }

            result.Add(Value);
            result.Add(Duration);
            return result;
        }
#endif
    }
}