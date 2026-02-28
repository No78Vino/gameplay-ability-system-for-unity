namespace GAS.Runtime
{
    /// <summary>  
    /// MMC计算上下文，封装计算所需的所有OOP信息，用户侧不感知任何ECS/DOTS类型。  
    /// </summary>  
    public sealed class MmcContext
    {
        /// <summary>施法者ASC（GameplayEffect的来源）</summary>  
        public AbilitySystemCell Source { get; internal set; }

        /// <summary>目标ASC（GameplayEffect的目标）</summary>  
        public AbilitySystemCell Target { get; internal set; }

        /// <summary>GameplayEffect的OOP包装</summary>  
        public GameplayEffectSpec EffectSpec { get; internal set; }
    }
}