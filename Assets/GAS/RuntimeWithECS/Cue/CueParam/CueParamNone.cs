using System.Collections.Generic;
using cfg;

namespace GAS.Runtime
{
    public class CueParamNone:ICueParameter
    {
        public CueParamNone()
        {
        }
        
        public void LoadConfigParameterData(CueLogic cfgCueLogic)
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