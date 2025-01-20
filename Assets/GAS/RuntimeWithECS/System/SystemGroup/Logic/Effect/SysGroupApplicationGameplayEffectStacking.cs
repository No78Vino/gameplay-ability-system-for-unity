using GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.SystemGroup
{
    [UpdateInGroup(typeof(SysGroupDurationalEffect))]
    [UpdateBefore(typeof(SysInitDuartionalEffect))]
    public partial class SysGroupApplicationGameplayEffectStacking : ComponentSystemGroup
    {
    }
}