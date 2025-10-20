using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpLogic))]
    [UpdateAfter(typeof(SysGrpTryApplyEffect))]
    public partial class SysGrpInstantEffect : ComponentSystemGroup
    {
    }
}