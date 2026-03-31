using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGCheckApplyEffect))]
    [UpdateBefore(typeof(SCheckApplyEnd))]
    public partial struct SCheckImmunityTags : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectImmunityTags>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<WipCheckApplyEffect>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();

            foreach (var (_, _, inUsage, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipCheckApplyEffect>,
                         RefRO<CEffectInUsage>
                     >().WithAll<CEffectImmunityTags>().WithEntityAccess())
            {
                var asc = inUsage.ValueRO.Target;
                var requirement = state.EntityManager.GetComponentData<CEffectImmunityTags>(ge).requirement;

                if(!tagMap.AscEvaluateTagRequirement(state.EntityManager, asc, requirement)) continue;

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
