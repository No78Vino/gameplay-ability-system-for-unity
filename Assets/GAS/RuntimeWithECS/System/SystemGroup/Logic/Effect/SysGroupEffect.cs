using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupLogic))]
    [UpdateAfter(typeof(SysGroupAbility))]
    public partial class SysGroupEffect : ComponentSystemGroup
    {
    }
}