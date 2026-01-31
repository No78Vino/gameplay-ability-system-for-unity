using System.Collections.Generic;

namespace GAS.Runtime
{
    public class XParamMMCScalable:XParam
    {
        public float K;
        public float B;
        
        public void SetK(float value)
        {
            K = value;
        }
        
        public void SetB(float value)
        {
            B = value;
        }
        
        public XParamMMCScalable(float k,float b)
        {
            K = k;
            B = b;
        }
        
        public XParamMMCScalable()
        {
            K = 0;
            B = 0;
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