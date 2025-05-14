///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

namespace GAS.Runtime
{
    public static class GEN_CueCode
    {
        public const string CUE_CueLog = "GAS.Runtime.CueLog";

        public static void LoadCueType()
        {
            var CueLog = typeof(GAS.Runtime.GameplayCueLog);
            CueHelper.RegisterCue(CUE_CueLog, CueLog);
        }
    }
}
