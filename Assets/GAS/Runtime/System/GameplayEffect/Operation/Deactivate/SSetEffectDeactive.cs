using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGDeactivateEffect))]
        [UpdateBefore( typeof(SDeactivateEnd))]
    public partial struct SSetEffectDeactive : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GlobalTimer>();
            state.RequireForUpdate<WipDeactivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<CDuration>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var globalTimer = SystemAPI.GetSingleton<GlobalTimer>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (_, _, inUsage, durationComp,ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipDeactivateEffect>,
                         RefRO<CEffectInUsage>,
                         RefRW<CDuration>>().WithEntityAccess())
            {
                
                var duration = durationComp.ValueRW;
                duration.active = false;
                durationComp.ValueRW = duration;
                
                // EffectContainer脏标记
                var targetAsc = inUsage.ValueRO.Target;
                GASEventCenter.InvokeOnGameplayEffectContainerIsDirty(targetAsc);
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