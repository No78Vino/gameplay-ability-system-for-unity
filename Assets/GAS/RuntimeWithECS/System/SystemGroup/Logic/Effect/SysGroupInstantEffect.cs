using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup
{
    
    [UpdateInGroup(typeof(SysGroupLogic))]
    [UpdateAfter(typeof(SysGroupTryApplyEffect))]
    public partial class SysGroupInstantEffect : ComponentSystemGroup
    {
    }
}