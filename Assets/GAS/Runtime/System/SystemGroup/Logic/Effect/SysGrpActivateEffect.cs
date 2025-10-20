using GAS.Runtime;
using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpDurationalEffect))]
    [UpdateAfter(typeof(SysInvokeEffectContainerIsDirtyEvent))]
    public partial class SysGrpActivateEffect : ComponentSystemGroup
    {
    }
}