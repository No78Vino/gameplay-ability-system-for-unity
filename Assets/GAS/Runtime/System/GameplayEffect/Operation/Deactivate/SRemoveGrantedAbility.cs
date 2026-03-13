// 文件: Assets/GAS/Runtime/System/GameplayEffect/Operation/Deactivate/SRemoveGrantedAbility.cs  
using Unity.Burst;  
using Unity.Entities;  
  
namespace GAS.Runtime  
{  
    [UpdateInGroup(typeof(SGDeactivateEffect))]  
    [UpdateBefore(typeof(SDeactivateEnd))]  
    public partial struct SRemoveGrantedAbility : ISystem  
    {  
        [BurstCompile]  
        public void OnCreate(ref SystemState state)  
        {  
            state.RequireForUpdate<WipDeactivateEffect>();  
            state.RequireForUpdate<CEffectInstance>();  
            state.RequireForUpdate<MCGrantedAbility>();  
            state.RequireForUpdate<CEffectInUsage>();  
        }  
  
        public void OnUpdate(ref SystemState state)  
        {  
            foreach (var (_, _, grantedAbilityComp, inUsage, ge) in  
                     SystemAPI.Query<  
                         RefRO<CEffectInstance>,  
                         RefRO<WipDeactivateEffect>,  
                         MCGrantedAbility,  
                         RefRO<CEffectInUsage>>().WithEntityAccess())  
            {  
                if (!state.EntityManager.HasComponent<MCGrantedAbilityRuntime>(ge)) continue;  
  
                var runtime = state.EntityManager.GetComponentData<MCGrantedAbilityRuntime>(ge);  
                if (runtime.GrantedAbilityEntities == null) continue;  
  
                var grantedAbilities = grantedAbilityComp.GrantedAbilities;  
  
                for (int i = 0; i < grantedAbilities.Length; i++)  
                {  
                    if (grantedAbilities[i].DeactivationPolicy != GrantedAbilityDeactivationPolicy.SyncWithEffect)  
                        continue;  
  
                    var abilityEntity = runtime.GrantedAbilityEntities[i];  
                    if (abilityEntity == Entity.Null) continue;  
                    if (!state.EntityManager.HasComponent<CAbilityActive>(abilityEntity)) continue;  
  
                    // 添加取消标记，由下一帧SGAbility的STryCancelAbility处理  
                    state.EntityManager.AddComponent<CAbilityInTryCancel>(abilityEntity);  
                }  
            }  
        }  
  
        [BurstCompile]  
        public void OnDestroy(ref SystemState state)  
        {  
        }  
    }  
}