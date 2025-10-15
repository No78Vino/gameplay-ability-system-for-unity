using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupLogic))]
    [UpdateAfter(typeof(SysGroupTryApplyEffect))]
    public partial class SysGroupInstantEffect : ComponentSystemGroup
    {
    }
}