using System;
using System.Collections.Generic;

namespace GAS.RuntimeWithECS
{
    public class AbilityParamArrayInt: IAbilityParam
    {
        private int[] _value;
        public int[] Value => _value;
        
        public void SetValue(int[] value)
        {
            _value = value;
        }
        
        public AbilityParamArrayInt(int[] value)
        {
            _value = value;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                _value = Array.Empty<int>();
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                _value = Array.Empty<int>();
                return;
            }

            var strArray = strData.Split(';');
            _value = new int[strArray.Length];
            for (var i = 0; i < strArray.Length; i++)
                if (int.TryParse(strArray[i], out var val))
                    _value[i] = val;
                else
                    _value[i] = 0;
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