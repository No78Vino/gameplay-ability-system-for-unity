using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysCheckEffectOngoingActive))]
    public partial struct SysEffectTicker : ISystem
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
            foreach (var (duration, inUsage, geEntity) in SystemAPI.Query<RefRW<ComDuration>, RefRW<ComInUsage>>()
                         .WithEntityAccess())
            {
                // 是否已到达持续时间
                if (duration.ValueRO.duration <= 0) continue;
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