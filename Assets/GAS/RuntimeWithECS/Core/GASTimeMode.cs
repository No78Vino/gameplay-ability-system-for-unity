namespace GAS.RuntimeWithECS.Core
{
    public enum GASTimeMode
    {
        Default,          // 默认使用项目设置的帧率计时
        Turn,             // 外部手动更新，适用于回合制游戏,或者自定义帧数（后期服务器同步帧率，会使用Turn模式）
    }
}