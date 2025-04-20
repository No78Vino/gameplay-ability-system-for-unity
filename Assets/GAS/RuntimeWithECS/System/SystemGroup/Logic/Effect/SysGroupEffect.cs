using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGroupLogic))]
    [UpdateAfter(typeof(SysGroupAbility))]
    public partial class SysGroupEffect : ComponentSystemGroup
    {
    }
}