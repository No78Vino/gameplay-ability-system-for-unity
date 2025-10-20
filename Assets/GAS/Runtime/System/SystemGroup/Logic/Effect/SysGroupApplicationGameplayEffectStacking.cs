using GAS.Runtime;
using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGroupDurationalEffect))]
    [UpdateBefore(typeof(SInitDuartionalEffect))]
    public partial class SysGroupApplicationGameplayEffectStacking : ComponentSystemGroup
    {
    }
}