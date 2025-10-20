using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpLogic))]
    [UpdateAfter(typeof(SysGrpEffect))]
    public partial class SysGrpAttribute : ComponentSystemGroup
    {
    }
}