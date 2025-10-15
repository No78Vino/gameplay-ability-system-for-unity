using System.Collections.Generic;

namespace GAS.Runtime
{
    public class AbilityParamNone: IAbilityParam
    {
        private static AbilityParamNone _instance;
        
        public static AbilityParamNone None => _instance ??= new AbilityParamNone();
        
        private AbilityParamNone()
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