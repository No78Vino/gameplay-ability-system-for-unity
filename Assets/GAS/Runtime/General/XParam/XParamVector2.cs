using System.Collections.Generic;

namespace GAS.Runtime
{
    public class XParamVector2: XParam
    {
        private UnityEngine.Vector2 _value;
        public UnityEngine.Vector2 Value => _value;
        
        public void SetValue(UnityEngine.Vector2 value)
        {
            _value = value;
        }
        
        public XParamVector2(UnityEngine.Vector2 value)
        {
            _value = value;
        }
        
        public XParamVector2()
        {
            _value = UnityEngine.Vector2.zero;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                _value = UnityEngine.Vector2.zero;
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                _value = UnityEngine.Vector2.zero;
                return;
            }
            var dataParts = strData.Split(';');
            if (dataParts.Length < 2 || 
                !float.TryParse(dataParts[0], out var x) || 
                !float.TryParse(dataParts[1], out var y))
            {
                _value = UnityEngine.Vector2.zero;
                return;
            }
            _value = new UnityEngine.Vector2(x, y);
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object> { $"{_value.x};{_value.y}" };
            return result;
        }
#endif
    }
}