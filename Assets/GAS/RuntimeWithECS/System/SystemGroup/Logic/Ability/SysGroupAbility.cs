using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupLogic))]
    [UpdateAfter(typeof(SGlobalTimer))]
    public partial class SysGroupAbility : ComponentSystemGroup
    {
    }
}