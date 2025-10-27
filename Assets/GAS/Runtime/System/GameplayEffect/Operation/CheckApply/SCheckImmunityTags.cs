using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGCheckApplyEffect))]
    public partial struct SCheckImmunityTags : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectImmunityTags>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();

            foreach (var (_, immunityTags, inUsage, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<CEffectImmunityTags>,
                         RefRO<CEffectInUsage>
                     >().WithEntityAccess())
            {
                var asc = inUsage.ValueRO.Target;
                var tags = immunityTags.ValueRO.tags;
                if (tagMap.AscHasAnyTags(state.EntityManager, asc, tags)) continue;
                ecb.RemoveComponent<CEffectInstance>(ge);
                ecb.AddComponent<CEffectDestroy>(ge);
                // TODO 触发免疫Cue
                
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