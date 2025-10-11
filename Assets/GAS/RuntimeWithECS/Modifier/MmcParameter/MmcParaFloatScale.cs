using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS;

namespace GAS.Runtime
{
    [Serializable]
    public class MmcParaFloatScale : IMmcParameter
    {
        public float k;
        public float b;

        public MmcParaFloatScale()
        {
            k = 1;
            b = 0;
        }

        public MmcParaFloatScale(float k, float b)
        {
            this.k = k;
            this.b = b;
        }
        
        public void SetK(float k) =>this.k = k;
        public void SetB(float b) =>this.b = b;
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData.Count > 1)
                k = Convert.ToSingle(paramData[0]);
            else
                k = 1;

            if (paramData.Count > 2)
                b = Convert.ToSingle(paramData[1]);
            else
                b = 0;
        }

        public List<object> EncodeExcelData()
        {
            return new List<object> { k, b };
        }
#endif
    }
}