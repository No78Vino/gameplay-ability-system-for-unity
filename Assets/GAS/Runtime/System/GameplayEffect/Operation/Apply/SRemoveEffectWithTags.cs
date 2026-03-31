using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGApplyEffect))]
    [UpdateBefore(typeof(SApplyEnd))]
    public partial struct SRemoveEffectWithTags : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<CRemoveEffectWithTags>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<WipApplyEffect>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();
            
            foreach (var (_,_, inUsage, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipApplyEffect>,
                         RefRO<CEffectInUsage>
                     >().WithAll<CRemoveEffectWithTags>().WithEntityAccess())
            {
                var requirement = state.EntityManager.GetComponentData<CRemoveEffectWithTags>(ge).requirement;

                var asc = inUsage.ValueRO.Target;

                var geBuffer = SystemAPI.GetBuffer<BGameplayEffect>(asc);
                for (var i = geBuffer.Length - 1; i >= 0; i--)
                {
                    var geWillRemove = geBuffer[i].GameplayEffect;
                    var hasRemoveTag = tagMap.EffectEvaluateTagRequirement(state.EntityManager, geWillRemove, requirement);
                    if (!hasRemoveTag) continue;
                    
                    ecb.AddComponent<WipDeactivateEffect>(geWillRemove);
                    ecb.AddComponent<WipRemoveEffect>(geWillRemove);
                }
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
