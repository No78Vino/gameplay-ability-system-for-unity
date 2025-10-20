using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpLogic))]
    [UpdateAfter(typeof(SysGrpAbility))]
    public partial class SysGrpEffect : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpEffect))]
    public partial class SysGrpTryApplyEffect:ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpEffect))]
    [UpdateAfter(typeof(SysGrpTryApplyEffect))]
    public partial class SysGrpInstantEffect : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpEffect))]
    [UpdateAfter(typeof(SysGrpInstantEffect))]
    public partial class SysGrpDurationalEffect : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpEffect))]
    [UpdateAfter(typeof(SysGrpDurationalEffect))]
    public partial class SysGrpActivateEffect : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpEffect))]
    [UpdateAfter(typeof(SysGrpActivateEffect))]
    public partial class SysGrpDeactivateEffect : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpEffect))]
    [UpdateAfter(typeof(SysGrpDeactivateEffect))]
    public partial class SysGrpKillEffect : ComponentSystemGroup
    {
    }
}