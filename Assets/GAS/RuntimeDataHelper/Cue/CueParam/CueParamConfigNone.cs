using System;
using GAS.Runtime;

namespace GAS.Editor
{
    [Serializable]
    public class CueParamConfigNone : CueParamConfigBase<CueParamNone>
    {
        public override ICueParameter GetConfig()
        {
            return new CueParamNone();
        }
    }
}