using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupLogic))]
    [UpdateAfter(typeof(SysGroupEffect))]
    public partial class SysGroupAttribute : ComponentSystemGroup
    {
    }
}