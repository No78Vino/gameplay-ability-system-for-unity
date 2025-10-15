using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupLogic))]
    [UpdateAfter(typeof(SysGroupDurationalEffect))]
    public partial class SysGroupApplicationEnd : ComponentSystemGroup
    {
    }
}