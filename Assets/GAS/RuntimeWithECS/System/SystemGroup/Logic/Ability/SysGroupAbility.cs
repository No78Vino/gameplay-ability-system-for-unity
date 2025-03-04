using GAS.RuntimeWithECS.System.SystemGroup.LogicTick;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup
{
    [UpdateInGroup(typeof(SysGroupTickAbility))]
    public partial class SysGroupAbility : ComponentSystemGroup
    {
    }
}