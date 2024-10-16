using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect.Component
{
    /// <summary>
    /// GE的计时器
    /// </summary>
    public struct ComEffectTicker : IComponentData
    {
        /// <summary>
        /// 开始计时时间
        /// </summary>
        public int StartTime;

        /// <summary>
        /// 间隔计时
        /// 如果GE是有period的会使用到
        /// </summary>
        public int PeriodTickCount;
    }
}