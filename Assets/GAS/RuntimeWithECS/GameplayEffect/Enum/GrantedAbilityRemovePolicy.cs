using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    /// <summary>
    /// 授予能力的移除策略
    /// </summary>
    public enum GrantedAbilityRemovePolicy
    {
        /// <summary>
        /// 不移除
        /// </summary>
        [LabelText("不移除", SdfIconType.Joystick)]
        None,

        /// <summary>
        /// 同步GE，GE移除时移除
        /// </summary>
        [LabelText("同步GE", SdfIconType.Robot)]
        SyncWithEffect,

        /// <summary>
        /// 能力结束时自己移除
        /// </summary>
        [LabelText("能力结束时移除", SdfIconType.LightningChargeFill)]
        WhenEnd,

        /// <summary>
        /// 能力取消时自己移除
        /// </summary>
        [LabelText("能力取消时移除", SdfIconType.LightningChargeFill)]
        WhenCancel,

        /// <summary>
        /// 能力结束或取消时自己移除
        /// </summary>
        [LabelText("能力结束或取消时移除", SdfIconType.LightningChargeFill)]
        WhenCancelOrEnd,
    }
}