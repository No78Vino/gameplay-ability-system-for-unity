using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public class XParamMMCScalable : XParam
    {
        public XParamMMCScalable(float k, float b)
        {
            K = k;
            B = b;
        }

        public XParamMMCScalable()
        {
            K = 0;
            B = 0;
        }

        [ShowInInspector]
        [BeanField(nameof(SetK))]
        public float K { get; private set; }

        [ShowInInspector]
        [BeanField(nameof(SetB))]
        public float B { get; private set; }

        public void SetK(float value)
        {
            K = value;
        }

        public void SetB(float value)
        {
            B = value;
        }

#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData == null || paramData.Count == 0)
            {
                K = 0;
                B = 0;
                return;
            }

            K = paramData[0] as float? ?? 0;

            if (paramData.Count > 1)
                B = paramData[1] as float? ?? 0;
        }

        public List<object> EncodeExcelData()
        {
            var result = new List<object> { K, B };
            return result;
        }
#endif
    }
}