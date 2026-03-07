using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    [Serializable]
    public class MmcParaFloatScale : XParam
    {
        [ShowInInspector]
        [BeanField(nameof(SetK))]
        public float K { get; private set; }

        [ShowInInspector]
        [BeanField(nameof(SetB))]
        public float B  { get; private set; }

        public MmcParaFloatScale()
        {
            K = 1;
            B = 0;
        }

        public MmcParaFloatScale(float k, float b)
        {
            K = k;
            B = b;
        }
        
        public void SetK(float k) =>K = k;
        public void SetB(float b) =>B = b;
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData.Count > 1)
                K = Convert.ToSingle(paramData[0]);
            else
                K = 1;

            if (paramData.Count > 2)
                B = Convert.ToSingle(paramData[1]);
            else
                B = 0;
        }

        public List<object> EncodeExcelData()
        {
            return new List<object> { K, B };
        }
#endif
    }
}