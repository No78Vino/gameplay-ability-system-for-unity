using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup.LogicTick
{
    [UpdateInGroup(typeof(SysGroupLogicTick))]
    public partial class SysGroupTickAbility : ComponentSystemGroup
    {
    }
}