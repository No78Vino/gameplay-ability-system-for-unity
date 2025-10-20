using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpLogic))]
    [UpdateAfter(typeof(SGlobalTimer))]
    public partial class SysGrpAbility : ComponentSystemGroup
    {
    }
}