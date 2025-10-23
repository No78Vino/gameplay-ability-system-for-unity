using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SGLogic))]
    public partial class SysGrpLogicTick : ComponentSystemGroup
    {
    }
}