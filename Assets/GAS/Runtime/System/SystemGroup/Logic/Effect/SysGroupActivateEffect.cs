using GAS.Runtime;
using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupDurationalEffect))]
    [UpdateAfter(typeof(SysInvokeEffectContainerIsDirtyEvent))]
    public partial class SysGroupActivateEffect : ComponentSystemGroup
    {
    }
}