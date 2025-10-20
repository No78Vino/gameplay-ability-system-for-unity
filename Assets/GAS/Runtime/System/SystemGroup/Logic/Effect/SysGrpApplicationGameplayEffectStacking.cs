using GAS.Runtime;
using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpDurationalEffect))]
    [UpdateBefore(typeof(SInitDuartionalEffect))]
    public partial class SysGrpApplicationGameplayEffectStacking : ComponentSystemGroup
    {
    }
}