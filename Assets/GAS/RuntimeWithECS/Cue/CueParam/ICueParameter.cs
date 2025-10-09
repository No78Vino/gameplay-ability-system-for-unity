using System.Collections.Generic;

namespace GAS.Runtime
{
    public interface ICueParameter
    {
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData);
        public List<object> EncodeExcelData();
#endif
    }
}