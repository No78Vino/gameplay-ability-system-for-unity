using GAS.Runtime;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup.LogicTick
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SysGroupLogic))]
    public partial class SysGroupLogicTick : ComponentSystemGroup
    {
    }
}