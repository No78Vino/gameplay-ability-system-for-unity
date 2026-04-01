using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGCheckActivateEffect))]
    public partial struct SCheckEffectActive : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipCheckActiveEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<CDuration>();
            state.RequireForUpdate<SingletonGameplayTagMap>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();
            
            foreach (var (_,_,_, inUsage, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipCheckActiveEffect>,
                         RefRO<CDuration>,
                         RefRO<CEffectInUsage>
                     >().WithEntityAccess())
            {
                var asc = inUsage.ValueRO.Target;
                var hasRequirement = state.EntityManager.HasComponent<COngoingRequiredTags>(ge);

                if (hasRequirement)
                {
                    var requirement = state.EntityManager.GetComponentData<COngoingRequiredTags>(ge).requirement;

                    if (tagMap.AscEvaluateTagRequirement(state.EntityManager, asc, requirement))
                    {
                        // 分配到激活阶段 Activate Effect
                        ecb.AddComponent<WipActivateEffect>(ge);
                    }
                    else
                    {
                        // 分配到失活阶段 Deactivate Effect
                        ecb.AddComponent<WipDeactivateEffect>(ge);
                    }
                }
                else
                {
                    // 分配到激活阶段 Activate Effect
                    ecb.AddComponent<WipActivateEffect>(ge);
                }

                // 完成检查，移除标记组件
                ecb.RemoveComponent<WipCheckActiveEffect>(ge);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
