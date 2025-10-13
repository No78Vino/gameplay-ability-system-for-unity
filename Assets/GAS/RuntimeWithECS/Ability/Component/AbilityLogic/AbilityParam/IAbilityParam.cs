using System.Collections.Generic;

namespace GAS.Runtime
{
    public interface IAbilityParam
    {
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData);
        public List<object> EncodeExcelData();
#endif
    }
}