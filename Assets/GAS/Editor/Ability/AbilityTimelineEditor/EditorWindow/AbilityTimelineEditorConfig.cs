#if UNITY_EDITOR
namespace GAS.Editor
{
    using GAS.General;

    public class AbilityTimelineEditorConfig
    {
        public float FrameUnitWidth = 10;
        public const float StandardFrameUnitWidth = 0.25f;
        public const int MaxFrameUnitLevel= 50;
        public const float MinTimerShaftFrameDrawStep = 10;
        public int DefaultFrameRate => GASTimer.FrameRate;
    }
}
#endif