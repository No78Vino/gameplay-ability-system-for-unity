using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public enum AttributeCaptureType
    {
        [LabelText(SdfIconType.Watch, Text = "追踪")]
        Track,
        [LabelText(SdfIconType.Camera, Text = "快照")]
        SnapShot
    }
}