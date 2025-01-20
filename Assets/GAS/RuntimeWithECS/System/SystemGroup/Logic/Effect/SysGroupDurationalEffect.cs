using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup
{
    [UpdateInGroup(typeof(SysGroupLogic))]
    [UpdateAfter(typeof(SysGroupInstantEffect))]
    public partial class SysGroupDurationalEffect : ComponentSystemGroup
    {
    }
}