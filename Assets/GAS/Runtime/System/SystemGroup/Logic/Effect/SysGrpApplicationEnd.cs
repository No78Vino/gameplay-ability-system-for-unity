using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpLogic))]
    [UpdateAfter(typeof(SysGrpDurationalEffect))]
    public partial class SysGrpApplicationEnd : ComponentSystemGroup
    {
    }
}