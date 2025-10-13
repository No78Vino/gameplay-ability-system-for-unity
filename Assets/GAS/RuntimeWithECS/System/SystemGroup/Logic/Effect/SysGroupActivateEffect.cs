using GAS.Runtime.System.GameplayEffect.PhaseDurationalEffect;
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