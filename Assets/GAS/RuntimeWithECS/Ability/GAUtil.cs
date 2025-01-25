namespace GAS.RuntimeWithECS.Ability
{
    public enum AbilityActivationResult
    {
        Success,
        FailHasActivated,
        FailTagRequirement,
        FailCost,
        FailCooldown,
        FailOtherReason
    }
    
    /// <summary>
    ///  Gameplay Ability Utility
    ///  游戏能力工具类,对应原本的AbilitySpec
    ///  原本Mono版本里的所有ability自身的功能性方法全部转为静态方法
    ///  Entity + Util方式代替Mono + AbilitySpec方式
    /// </summary>
    public static class GAUtil
    {
        
    }
}