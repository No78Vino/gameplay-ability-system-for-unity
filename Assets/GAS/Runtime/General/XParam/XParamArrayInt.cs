using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamArrayInt: XParam
    {
        [ShowInInspector]
        public int[] Value { get; private set; }

        public void SetValue(int[] value)
        {
            Value = value;
        }
        
        public XParamArrayInt()
        {
            Value = Array.Empty<int>();
        }
        
        public XParamArrayInt(int[] value)
        {
            Value = value;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                Value = Array.Empty<int>();
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                Value = Array.Empty<int>();
                return;
            }

            var strArray = strData.Split(';');
            Value = new int[strArray.Length];
            for (var i = 0; i < strArray.Length; i++)
                if (int.TryParse(strArray[i], out var val))
                    Value[i] = val;
                else
                    Value[i] = 0;
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