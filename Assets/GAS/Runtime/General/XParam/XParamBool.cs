using System.Collections.Generic;

namespace GAS.Runtime
{
    public class XParamBool
    {
        private bool _value;
        public bool Value => _value;
        
        public void SetValue(bool value)
        {
            _value = value;
        }
        
        public XParamBool(bool value)
        {
            _value = value;
        }
        
        public XParamBool()
        {
            _value = false;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                _value = false;
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                _value = false;
                return;
            }

            if (!bool.TryParse(strData, out _value))
            {
                _value = false;
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