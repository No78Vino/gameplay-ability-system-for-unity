///////////////////////////////////
//// This is a generated file. ////
////     Do not modify it.     ////
///////////////////////////////////

namespace GAS.Runtime
{
    public static class XCue
    {
        public const string CUE_CLCameraFovShake = "CLCameraFovShake";
        public const string CUE_CueLog = "CueLog";
        public const string CUE_CueLogging = "CueLogging";
        public const string CUE_CuePlayAnimator = "CuePlayAnimator";
        public const string CUE_CuePlaySound = "CuePlaySound";

        public static void LoadCueType()
        {
            var CLCameraFovShake = typeof(DemoForESC._Script.Gas.Cue.CLCameraFovShake);
            CueHelper.RegisterCue(CUE_CLCameraFovShake, CLCameraFovShake, typeof(GAS.Runtime.XParamFloat));
            var CueLog = typeof(GAS.Runtime.CueLog);
            CueHelper.RegisterCue(CUE_CueLog, CueLog, typeof(GAS.Runtime.XParamString));
            var CueLogging = typeof(GAS.Runtime.CueLogging);
            CueHelper.RegisterCue(CUE_CueLogging, CueLogging, typeof(GAS.Runtime.XParamLogging));
            var CuePlayAnimator = typeof(GAS.Runtime.CuePlayAnimator);
            CueHelper.RegisterCue(CUE_CuePlayAnimator, CuePlayAnimator, typeof(GAS.Runtime.XParamAnimator));
            var CuePlaySound = typeof(GAS.Runtime.CuePlaySound);
            CueHelper.RegisterCue(CUE_CuePlaySound, CuePlaySound, typeof(GAS.Runtime.XParamPlaySound));
        }
    }
}
