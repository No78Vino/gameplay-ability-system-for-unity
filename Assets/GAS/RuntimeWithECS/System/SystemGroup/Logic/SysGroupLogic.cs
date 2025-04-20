using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SysGroupLogic : ComponentSystemGroup
    {
    }
}