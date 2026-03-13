using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGLogic))]
    [UpdateAfter(typeof(SGAbility))]
    public partial class SGEffect : ComponentSystemGroup
    {
    }
    
    // Effect系统组别结构
    // 一级：Create->Operation->Destroy->Tick
    
    // 二级：
    // Create: InstantiateEffect
    // Operation: CheckApply,Apply,CheckActivate,Activate,Deactivate,Remove
    // Destroy: KillEffect
    // Tick: RunningEffect
    
    // 三级：
    // Apply：InstantEffect,DurationalEffect
    
    
    #region 一级系统组:Create->Operation->Destroy->Tick
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffect))]
    public partial class SGEffectCreate : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffect))]
    [UpdateAfter(typeof(SGEffectCreate))]
    public partial class SGEffectOperation : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffect))]
    [UpdateAfter(typeof(SGEffectOperation))]
    public partial class SGEffectDestroy : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffect))]
    [UpdateAfter(typeof(SGEffectDestroy))]
    public partial class SGEffectTick : ComponentSystemGroup
    {
    }
    
    #endregion


    #region 二级系统组Create: InstantiateEffect

    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffectCreate))]
    public partial class SGInstantiateEffect : ComponentSystemGroup
    {
    }

    #endregion
    
    #region 二级系统组Operation: CheckApply,Apply,CheckActivate,Activate,Deactivate,Remove
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffectOperation))]
    public partial class SGCheckApplyEffect : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffectOperation))]
    [UpdateAfter(typeof(SGCheckApplyEffect))]
    public partial class SGApplyEffect : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffectOperation))]
    [UpdateAfter(typeof(SGApplyEffect))]
    public partial class SGCheckActivateEffect : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffectOperation))]
    [UpdateAfter(typeof(SGCheckActivateEffect))]
    public partial class SGActivateEffect : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffectOperation))]
    [UpdateAfter(typeof(SGActivateEffect))]
    public partial class SGDeactivateEffect : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffectOperation))]
    [UpdateAfter(typeof(SGDeactivateEffect))]    
    public partial class SGRemoveEffect : ComponentSystemGroup
    {
    }

    #endregion
    
    #region 二级系统组Tick: RunningEffect

    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGEffectTick))]
    public partial class SGRunningEffect : ComponentSystemGroup
    {
    }

    #endregion
    
    #region 三级系统组Apply：InstantEffect,DurationalEffect

    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGApplyEffect))]
    public partial class SGInstantEffect : ComponentSystemGroup
    {
    }
    
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGApplyEffect))]
    public partial class SGDurationalEffect : ComponentSystemGroup
    {
    }

    #endregion
}