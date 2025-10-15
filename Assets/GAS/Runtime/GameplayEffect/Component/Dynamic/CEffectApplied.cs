using Unity.Entities;

namespace GAS.Runtime
{
    /// <summary>
    /// 有效Gameplay Effect的Tag
    /// 无效GE的情况有：
    /// 1.Application Required Tag校验不通过
    /// 2.Immunity Tag校验不通过
    /// 3.Application Condition校验不通过
    /// 4.Duration 超时过期
    /// 5.外部干涉，设置GE为无效
    /// </summary>
    public struct CEffectApplied : IComponentData
    {
    }
}