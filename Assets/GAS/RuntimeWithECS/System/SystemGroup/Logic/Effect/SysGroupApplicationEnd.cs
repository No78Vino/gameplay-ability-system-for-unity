using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup
{
    [UpdateInGroup(typeof(SysGroupLogic))]
    [UpdateAfter(typeof(SysGroupDurationalEffect))]
    public partial class SysGroupApplicationEnd : ComponentSystemGroup
    {
    }
}