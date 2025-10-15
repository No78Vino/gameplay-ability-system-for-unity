using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupLogic))]
    [UpdateAfter(typeof(SysGroupInstantEffect))]
    public partial class SysGroupDurationalEffect : ComponentSystemGroup
    {
    }
}