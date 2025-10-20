using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SysGrpLogic))]
    public partial class SysGrpLogicTick : ComponentSystemGroup
    {
    }
}