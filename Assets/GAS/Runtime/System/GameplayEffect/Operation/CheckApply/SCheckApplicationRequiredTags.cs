using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGCheckApplyEffect))]
    public partial struct SCheckApplicationRequiredTags : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SingletonGameplayTagMap>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CApplicationRequiredTags>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tagMap = SystemAPI.GetSingleton<SingletonGameplayTagMap>();

            foreach (var (_, applicationRequiredTags, inUsage, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<CApplicationRequiredTags>,
                         RefRO<CEffectInUsage>
                     >().WithEntityAccess())
            {
                var asc = inUsage.ValueRO.Target;
                var tags = applicationRequiredTags.ValueRO.tags;
                if (tagMap.AscHasAllTags(state.EntityManager, asc, tags)) continue;
                ecb.RemoveComponent<CEffectInstance>(ge);
                ecb.AddComponent<CEffectDestroy>(ge);
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