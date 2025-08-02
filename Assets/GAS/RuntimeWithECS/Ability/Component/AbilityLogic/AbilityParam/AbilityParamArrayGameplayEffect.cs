using System;
using System.Collections.Generic;

namespace GAS.RuntimeWithECS
{
    public class AbilityParamArrayGameplayEffect: IAbilityParam
    {
        private string[] _value;
        public string[] Value => _value;
        
        public void SetValue(string[] value)
        {
            _value = value;
        }
        
        public AbilityParamArrayGameplayEffect(string[] value)
        {
            _value = value;
        }
        
        public AbilityParamArrayGameplayEffect()
        {
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                _value = Array.Empty<string>();
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                _value = Array.Empty<string>();
                return;
            }

            var strArray = strData.Split(';');
            _value = new string[strArray.Length];
            for (var i = 0; i < strArray.Length; i++)
                _value[i] = strArray[i];
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