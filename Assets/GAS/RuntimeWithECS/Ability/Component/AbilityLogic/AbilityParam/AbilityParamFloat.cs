using System;
using System.Collections.Generic;

namespace GAS.RuntimeWithECS
{
    public class AbilityParamFloat: IAbilityParam
    {
        private float _value;
        public float Value => _value;
        
        public void SetValue(float value)
        {
            _value = value;
        }

        public AbilityParamFloat(float value)
        {
            _value = value;
        }
        
        public AbilityParamFloat()
        {
            _value = 0f;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                _value = 0f;
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                _value = 0f;
                return;
            }

            if (!float.TryParse(strData, out _value))
            {
                _value = 0f;
            }
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object> { _value.ToString() };
            return result;
        }
#endif
    }
}