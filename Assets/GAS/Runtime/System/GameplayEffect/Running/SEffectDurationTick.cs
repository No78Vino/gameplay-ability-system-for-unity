using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGRunningEffect))]
    [UpdateAfter(typeof(SEffectPeriodTick))]
    public partial struct SEffectDurationTick : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GlobalTimer>();
            state.RequireForUpdate<CDuration>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            var currentFrame = globalFrameTimer.ValueRO.Frame;
            var currentTurn = globalFrameTimer.ValueRO.Turn;
            var ecb = new EntityCommandBuffer(Allocator.Temp); 
            foreach (var (duration, _, inUsage, geEntity) in SystemAPI
                         .Query<RefRW<CDuration>, RefRO<CEffectInstance>, RefRO<CEffectInUsage>>()
                         .WithNone<CStacking>()
                         .WithEntityAccess())
            {
                // 过滤：
                // 1.持续时间无限的GE
                // 2.未激活的GE
                if (duration.ValueRO.duration <= 0 || !duration.ValueRO.active) continue;

                var durRO = duration.ValueRO;
                var countTime = duration.ValueRO.timeUnit == TimeUnit.Frame ? currentFrame : currentTurn;
                bool expired;
                if (duration.ValueRO.StopTickWhenDeactivated)
                    expired = countTime - durRO.lastActiveTime >= durRO.remianTime;
                else
                    expired = countTime - durRO.activeTime >= durRO.duration;
                
                
                // 过期的GE无效化，并销毁
                if (expired)
                {
                    ecb.AddComponent<WipDeactivateEffect>(geEntity);
                    ecb.AddComponent<WipRemoveEffect>(geEntity);
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