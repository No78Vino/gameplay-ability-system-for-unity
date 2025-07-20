using System.Collections.Generic;

namespace GAS.Runtime
{
    public class CueParamNone:ICueParameter
    {
        public CueParamNone()
        {
        }
        
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData)
        {
        }

        public List<object> EncodeExcelData()
        {
            return new List<object>();
        } 
#endif
    }
}