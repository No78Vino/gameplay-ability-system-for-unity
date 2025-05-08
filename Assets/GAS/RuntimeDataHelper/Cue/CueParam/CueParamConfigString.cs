using System;
using GAS.Runtime;
using Sirenix.OdinInspector;

namespace GAS.Editor
{
    [Serializable]
    public class CueParamConfigString : CueParamConfigBase<CueParamString>
    {
        [LabelText("文本")]
        public string message;
        public override ICueParameter GetConfig()
        {
            return new CueParamString(message);
        }
    }
}