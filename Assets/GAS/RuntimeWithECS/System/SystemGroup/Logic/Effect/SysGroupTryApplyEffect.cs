using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup
{
    [UpdateInGroup(typeof(SysGroupLogic))]
    public partial class SysGroupTryApplyEffect:ComponentSystemGroup
    {
    }
}