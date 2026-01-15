using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class ParamFloat:IExParameterBase
    {
        [LabelText("值")]
        [ShowInInspector]
        public float Value { get; private set; }
        
        public ParamFloat()
        {
            Value = 0;
        }
        
        public ParamFloat(float v)
        {
            Value = v;
        }

        public void SetValue(float v)
        {
            Value = v;
        }

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            Value = paramData.Count > 0 ? float.Parse(paramData[0].ToString()) : 0;
        }

        public List<object> EncodeExcelData()
        {
            var data = new List<object> { Value };
            return data;
        }
#endif
    }
}