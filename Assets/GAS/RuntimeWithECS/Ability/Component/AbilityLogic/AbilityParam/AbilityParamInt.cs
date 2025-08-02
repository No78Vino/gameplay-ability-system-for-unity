using System.Collections.Generic;

namespace GAS.RuntimeWithECS
{
    public class AbilityParamInt: IAbilityParam
    {
        private int _value;
        public int Value => _value;
        
        public void SetValue(int value)
        {
            _value = value;
        }
        
        public AbilityParamInt(int value)
        {
            _value = value;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                _value = 0;
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                _value = 0;
                return;
            }

            if (!int.TryParse(strData, out _value))
            {
                _value = 0;
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