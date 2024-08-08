namespace GAS.Runtime
{
    public static class GasRuntimeSettings
    {
        /// <summary>
        /// 是否禁用GameplayEffect的共享实例, 仅Editor下生效, 禁用后每次创建GameplayEffect都会创建新的实例方便调试
        /// </summary>
        public static bool DisableGameplayEffectSharedInstance { get; set; }
    }
}