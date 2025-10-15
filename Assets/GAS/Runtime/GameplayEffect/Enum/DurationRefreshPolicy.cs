using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    public enum DurationRefreshPolicy
    {
        [LabelText("不刷新持续时间", SdfIconType.XCircleFill)]
        NeverRefresh, //不刷新Effect的持续时间

        [LabelText("应用成功后刷新持续时间", SdfIconType.HourglassTop)]
        RefreshOnSuccessfulApplication //每次apply成功后刷新Effect的持续时间, denyOverflowApplication如果为True则多余的Apply不会刷新Duration
    }
}