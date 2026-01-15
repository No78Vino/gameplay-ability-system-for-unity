using System.Collections.Generic;

namespace GAS.Runtime
{
    public class ParamNone:IExParameterBase
    {
        public ParamNone()
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