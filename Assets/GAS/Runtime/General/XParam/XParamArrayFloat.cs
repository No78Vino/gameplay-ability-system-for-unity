using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamArrayFloat : XParam
    {
        public XParamArrayFloat()
        {
            Value = Array.Empty<float>();
        }
        
        public XParamArrayFloat(float[] value)
        {
            Value = value;
        }

        [ShowInInspector]
        [BeanField(nameof(SetValue))]
        public float[] Value { get; private set; }

        public void SetValue(float[] value)
        {
            Value = value;
        }

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                Value = Array.Empty<float>();
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                Value = Array.Empty<float>();
                return;
            }

            var strArray = strData.Split(';');
            Value = new float[strArray.Length];
            for (var i = 0; i < strArray.Length; i++)
                if (float.TryParse(strArray[i], out var val))
                    Value[i] = val;
                else
                    Value[i] = 0f;
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object>();
            if (Value == null || Value.Length == 0)
            {
                result.Add(string.Empty);
                return result;
            }

            var strData = string.Join(";", Value);
            result.Add(strData);
            return result;
        }
#endif
    }
}