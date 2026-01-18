#if UNITY_EDITOR
namespace GAS.Editor
{
    using GAS.General;

    public class AbilityTimelineEditorConfig
    {
        public float FrameUnitWidth = 10;
        public const float StandardFrameUnitWidth = 0.4f;
        public const int MaxFrameUnitLevel= 100;
        public const float MinTimerShaftFrameDrawStep = 10;
        public int DefaultFrameRate => GASTimer.FrameRate;
    }
}
#endif