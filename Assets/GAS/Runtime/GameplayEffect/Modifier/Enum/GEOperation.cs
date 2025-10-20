using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public enum GEOperation
    {
        [LabelText(SdfIconType.PlusLg, Text = "加")]
        Add = 0,

        [LabelText(SdfIconType.DashLg, Text = "减")]
        Minus = 3,

        [LabelText(SdfIconType.XLg, Text = "乘")]
        Multiply = 1,

        [LabelText(SdfIconType.SlashLg, Text = "除")]
        Divide = 4,

        [LabelText(SdfIconType.Pencil, Text = "替")]
        Override = 2,
    }
}