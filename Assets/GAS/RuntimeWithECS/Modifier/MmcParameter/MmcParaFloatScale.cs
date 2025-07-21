using System;
using System.Collections.Generic;

namespace GAS.RuntimeWithECS.Modifier.MmcParameter
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
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
            if (paramData.Count > 1 && paramData[0] is float kValue)
                k = kValue;
            else
                k = 1;

            if (paramData.Count > 2 && paramData[1] is float bValue)
                b = bValue;
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