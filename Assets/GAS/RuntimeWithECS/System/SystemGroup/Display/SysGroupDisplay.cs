using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SysGroupLogicTick))]
    public partial class SysGroupDisplay : ComponentSystemGroup
    {
    }
}