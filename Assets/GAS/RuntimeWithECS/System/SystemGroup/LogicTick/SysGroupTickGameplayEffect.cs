using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupLogicTick))]
    public partial class SysGroupTickGameplayEffect : ComponentSystemGroup
    {
    }
}