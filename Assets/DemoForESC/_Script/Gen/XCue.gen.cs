///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

namespace GAS.Runtime
{
    public static class XCue
    {
        public const string CUE_CLCameraFovShake = "CLCameraFovShake";
        public const string CUE_GameplayCueLog = "GameplayCueLog";

        public static void LoadCueType()
        {
            var CLCameraFovShake = typeof(DemoForESC._Script.Gas.Cue.CLCameraFovShake);
            CueHelper.RegisterCue(CUE_CLCameraFovShake, CLCameraFovShake, typeof(GAS.Runtime.ParamFloat));
            var GameplayCueLog = typeof(GAS.Runtime.GameplayCueLog);
            CueHelper.RegisterCue(CUE_GameplayCueLog, GameplayCueLog, typeof(GAS.Runtime.ParamString));
        }
    }
}
