using GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup
{
    [UpdateInGroup(typeof(SysGroupDurationalEffect))]
    [UpdateAfter(typeof(SysInvokeEffectContainerIsDirtyEvent))]
    public partial class SysGroupActivateEffect : ComponentSystemGroup
    {
    }
}