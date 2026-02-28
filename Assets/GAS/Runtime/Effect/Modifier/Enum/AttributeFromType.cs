using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public enum AttributeFromType
    {
        [LabelText(SdfIconType.Eye, Text = "来源")]
        Source,
        [LabelText(SdfIconType.Star, Text = "目标")]
        Target
    }
}