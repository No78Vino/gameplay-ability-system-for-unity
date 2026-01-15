using System.Collections.Generic;

namespace GAS.Runtime
{
    public class XParamVector3: IExParameterBase
    {
        private UnityEngine.Vector3 _value;
        public UnityEngine.Vector3 Value => _value;
        
        public XParamVector3()
        {
            _value = UnityEngine.Vector3.zero;
        }
        
        public XParamVector3(UnityEngine.Vector3 value)
        {
            _value = value;
        }
        public void SetValue(UnityEngine.Vector3 value)
        {
            _value = value;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                _value = UnityEngine.Vector3.zero;
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                _value = UnityEngine.Vector3.zero;
                return;
            }

            var parts = strData.Split(';');
            if (parts.Length != 3 ||
                !float.TryParse(parts[0], out var x) ||
                !float.TryParse(parts[1], out var y) ||
                !float.TryParse(parts[2], out var z))
            {
                _value = UnityEngine.Vector3.zero;
                return;
            }

            _value = new UnityEngine.Vector3(x, y, z);
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object>
            {
                $"{_value.x},{_value.y},{_value.z}"
            };
            return result;
        }
#endif
    }
}