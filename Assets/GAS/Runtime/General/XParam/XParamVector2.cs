using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamVector2: XParam
    {
        [LabelText("值")]
        [ShowInInspector]
        [BeanField(nameof(SetValue), Comment = "值")]
        public UnityEngine.Vector2 Value { get; private set; }
        
        public void SetValue(UnityEngine.Vector2 value)
        {
            Value = value;
        }
        
        public XParamVector2(UnityEngine.Vector2 value)
        {
            Value = value;
        }
        
        public XParamVector2()
        {
            Value = UnityEngine.Vector2.zero;
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                Value = UnityEngine.Vector2.zero;
                return;
            }

            var strData = paramData[0] as string;
            if (string.IsNullOrEmpty(strData))
            {
                Value = UnityEngine.Vector2.zero;
                return;
            }
            var dataParts = strData.Split(';');
            if (dataParts.Length < 2 || 
                !float.TryParse(dataParts[0], out var x) || 
                !float.TryParse(dataParts[1], out var y))
            {
                Value = UnityEngine.Vector2.zero;
                return;
            }
            Value = new UnityEngine.Vector2(x, y);
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object> { $"{Value.x};{Value.y}" };
            return result;
        }
#endif
    }
}