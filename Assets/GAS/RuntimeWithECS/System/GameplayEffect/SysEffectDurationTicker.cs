using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysCheckEffectOngoingActive))]
    public partial struct SysEffectDurationTicker : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComDuration>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            var currentFrame = globalFrameTimer.ValueRO.Frame;
            var currentTurn = globalFrameTimer.ValueRO.Turn;
            foreach (var (duration, _, geEntity) in SystemAPI.Query<RefRW<ComDuration>, RefRW<ComInUsage>>()
                         .WithEntityAccess())
            {
                // 过滤已经不合法的GE
                if (!SystemAPI.IsComponentEnabled<ComInUsage>(geEntity)) continue;
                // 过滤持续时间无限的GE
                if (duration.ValueRO.duration <= 0) continue;
                // 过滤未激活的GE
                if (!duration.ValueRO.active) continue;
                
                var durRO = duration.ValueRO;
                var countTime = duration.ValueRO.timeUnit == TimeUnit.Frame ? currentFrame : currentTurn;
                bool timeEnough;
                if (duration.ValueRO.StopTickWhenDeactivated)
                    timeEnough = countTime - durRO.lastActiveTime < durRO.remianTime;
                else
                    timeEnough = countTime - durRO.activeTime < durRO.duration;
                    
                SystemAPI.SetComponentEnabled<ComInUsage>(geEntity,timeEnough);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}