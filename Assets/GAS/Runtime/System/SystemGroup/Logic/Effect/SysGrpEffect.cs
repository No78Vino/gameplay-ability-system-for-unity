using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpLogic))]
    [UpdateAfter(typeof(SysGrpAbility))]
    public partial class SysGrpEffect : ComponentSystemGroup
    {
    }
}