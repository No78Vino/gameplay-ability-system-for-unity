using Sirenix.OdinInspector;

namespace GAS.Runtime
{
    /// <summary>
    /// 授予能力的激活策略
    /// </summary>
    public enum GrantedAbilityActivationPolicy
    {
        /// <summary>
        /// 不激活, 等待用户调用ASC激活
        /// </summary>
        [LabelText("不激活", SdfIconType.Joystick)]
        None,

        /// <summary>
        /// 能力添加时激活（GE添加时激活）
        /// </summary>
        [LabelText("添加时激活", SdfIconType.LightningChargeFill)]
        WhenAdded,

        /// <summary>
        /// 同步GE激活时激活
        /// </summary>
        [LabelText("同步GE", SdfIconType.Robot)]
        SyncWithEffect,
    }
}