using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SysGrpLogicTick))]
    public partial class SysGrpDisplay : ComponentSystemGroup
    {
    }
}