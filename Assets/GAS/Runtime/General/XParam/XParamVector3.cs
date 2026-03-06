using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamVector3: XParam
    {
        [LabelText("值")]
        [ShowInInspector]
        [BeanField(nameof(SetValue), Comment = "值")]
        public UnityEngine.Vector3 Value { get; private set; }
        
        public XParamVector3()
        {
            Value = UnityEngine.Vector3.zero;
        }
        
        public XParamVector3(UnityEngine.Vector3 value)
        {
            Value = value;
        }
        public void SetValue(UnityEngine.Vector3 value)
        {
            Value = value;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                Value = UnityEngine.Vector3.zero;
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                Value = UnityEngine.Vector3.zero;
                return;
            }

            var parts = strData.Split(';');
            if (parts.Length != 3 ||
                !float.TryParse(parts[0], out var x) ||
                !float.TryParse(parts[1], out var y) ||
                !float.TryParse(parts[2], out var z))
            {
                Value = UnityEngine.Vector3.zero;
                return;
            }

            Value = new UnityEngine.Vector3(x, y, z);
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object>
            {
                $"{Value.x},{Value.y},{Value.z}"
            };
            return result;
        }
#endif
    }
}