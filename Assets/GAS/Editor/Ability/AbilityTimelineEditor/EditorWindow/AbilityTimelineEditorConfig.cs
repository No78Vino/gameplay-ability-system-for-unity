#if UNITY_EDITOR
namespace GAS.Editor
{
    using GAS.General;

    public class AbilityTimelineEditorConfig
    {
        public int FrameUnitWidth = 10;
        public const int StandardFrameUnitWidth = 1;
        public const int MaxFrameUnitLevel= 20;
        public const float MinTimerShaftFrameDrawStep = 5;
        public int DefaultFrameRate => GASTimer.FrameRate;
    }
}
#endif