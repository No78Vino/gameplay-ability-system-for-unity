using System.Collections.Generic;

namespace GAS.RuntimeWithECS.Modifier.MmcParameter
{
    public class MmcParamNone : IMmcParameter
    {
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