namespace GAS.RuntimeWithECS.Core
{
    public enum GASTimeMode
    {
        Default_60FPS,          // 默认60帧的计时更新频率
        CustomTickUpdate,       // 自定义帧的计时更新频率
        Manual,                 // 手动更新，适用于回合制游戏
    }
}