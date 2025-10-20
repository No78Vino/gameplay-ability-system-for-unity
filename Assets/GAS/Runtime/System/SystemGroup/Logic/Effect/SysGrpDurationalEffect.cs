using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpLogic))]
    [UpdateAfter(typeof(SysGrpInstantEffect))]
    public partial class SysGrpDurationalEffect : ComponentSystemGroup
    {
    }
}