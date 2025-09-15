using System.Collections.Generic;

namespace GAS.Runtime
{
    public interface ICueParameter
    {
        public void LoadConfigParameterData(cfg.CueLogic cfgCueLogic);
#if UNITY_EDITOR
        public void DecodeExcelData(List<object> paramData);
        public List<object> EncodeExcelData();
#endif
    }
}