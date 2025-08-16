///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

namespace GAS.Runtime
{
    public static class XCue
    {
        public const string CUE_GameplayCueLog = "GameplayCueLog";

        public static void LoadCueType()
        {
            var GameplayCueLog = typeof(GAS.Runtime.GameplayCueLog);
            CueHelper.RegisterCue(CUE_GameplayCueLog, GameplayCueLog);
        }
    }
}
