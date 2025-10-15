using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SysGroupLogic))]
    public partial class SysGroupLogicTick : ComponentSystemGroup
    {
    }
}