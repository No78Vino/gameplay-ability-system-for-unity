using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGApplyEffect))]
    public partial struct SRemoveEffectWithTags : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<CRemoveEffectWithTags>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();
            
            foreach (var (_, removeEffectWithTags, inUsage) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<CRemoveEffectWithTags>,
                         RefRO<CEffectInUsage>
                     >())
            {
                var tags = removeEffectWithTags.ValueRO.tags;
                if (tags.Length == 0) continue;
                
                var asc = inUsage.ValueRO.Target;
                var geBuff = SystemAPI.GetBuffer<BGameplayEffect>(asc);
                for (var i = geBuff.Length - 1; i >= 0; i--)
                {
                    var ge = geBuff[i].GameplayEffect;
                    var hasRemoveTag = tagMap.EffectHasAnyTags(state.EntityManager,ge,tags);
                    if (!hasRemoveTag) continue;
                    
                    ecb.AddComponent<CWaitingDeactivateEffect>(ge);
                    ecb.AddComponent<CWaitingRemoveEffect>(ge);
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