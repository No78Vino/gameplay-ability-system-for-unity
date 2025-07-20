using System;
using System.Collections.Generic;
using GAS.RuntimeWithECS.Cue;

namespace GAS.Editor
{
    public abstract class CueParamEditorBase
    {
        public abstract void DecodeExcelData(List<object> paramData);
        public abstract List<object> EncodeExcelData();
    }
    
    public abstract class CueParamEditorBase<T> : CueParamEditorBase where T : GameplayCueBase
    {
    }
}