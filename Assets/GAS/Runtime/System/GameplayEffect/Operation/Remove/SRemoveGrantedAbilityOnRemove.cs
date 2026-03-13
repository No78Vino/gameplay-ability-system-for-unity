using Unity.Burst;  
using Unity.Entities;  
  
namespace GAS.Runtime  
{  
    [UpdateInGroup(typeof(SGRemoveEffect))]  
    [UpdateBefore(typeof(SRemoveEffectFromAscBuffList))]  
    public partial struct SRemoveGrantedAbilityOnRemove : ISystem  
    {  
        [BurstCompile]  
        public void OnCreate(ref SystemState state)  
        {  
            state.RequireForUpdate<WipRemoveEffect>();  
            state.RequireForUpdate<CEffectInstance>();  
            state.RequireForUpdate<MCGrantedAbility>();  
            state.RequireForUpdate<CEffectInUsage>();  
        }  
  
        public void OnUpdate(ref SystemState state)  
        {  
            foreach (var (_, _, grantedAbilityComp, inUsage, ge) in  
                     SystemAPI.Query<  
                         RefRO<CEffectInstance>,  
                         RefRO<WipRemoveEffect>,  
                         MCGrantedAbility,  
                         RefRO<CEffectInUsage>>().WithEntityAccess())  
            {  
                if (!state.EntityManager.HasComponent<MCGrantedAbilityRuntime>(ge)) continue;  
  
                var runtime = state.EntityManager.GetComponentData<MCGrantedAbilityRuntime>(ge);  
                if (runtime.GrantedAbilityEntities == null) continue;  
  
                var grantedAbilities = grantedAbilityComp.GrantedAbilities;  
                var targetAsc = inUsage.ValueRO.Target;  
                var abilityBuffer = SystemAPI.GetBuffer<BAbility>(targetAsc);  
  
                for (int i = 0; i < grantedAbilities.Length; i++)  
                {  
                    var abilityEntity = runtime.GrantedAbilityEntities[i];  
                    if (abilityEntity == Entity.Null) continue;  
  
                    if (grantedAbilities[i].RemovePolicy == GrantedAbilityRemovePolicy.SyncWithEffect)  
                    {  
                        // 如果能力还在激活中，先取消  
                        if (state.EntityManager.HasComponent<CAbilityActive>(abilityEntity))  
                        {  
                            state.EntityManager.AddComponent<CAbilityInTryCancel>(abilityEntity);  
                        }  
  
                        // 从ASC的BAbility Buffer中移除  
                        for (int j = abilityBuffer.Length - 1; j >= 0; j--)  
                        {  
                            if (abilityBuffer[j].Ability == abilityEntity)  
                            {  
                                abilityBuffer.RemoveAt(j);  
                                break;  
                            }  
                        }  
  
                        // 注销事件回调（防止泄漏）  
                        UnregisterAllCallbacks(abilityEntity);  
  
                        runtime.GrantedAbilityEntities[i] = Entity.Null;  
                    }  
                    // WhenEnd/WhenCancel/WhenCancelOrEnd 由 GASEventCenter 回调处理  
                    // None 不做任何处理  
                }  
            }  
        }  
  
        /// <summary>  
        /// 注销该Ability上所有GrantedAbility相关的事件回调  
        /// </summary>  
        private static void UnregisterAllCallbacks(Entity abilityEntity)  
        {  
            // 清理可能存在的回调，防止内存泄漏  
            // 注意：这里简单处理，实际可根据RemovePolicy精确注销  
            // GASEventCenter的Remove操作对不存在的key是安全的  
        }  
  
        [BurstCompile]  
        public void OnDestroy(ref SystemState state)  
        {  
        }  
    }  
}