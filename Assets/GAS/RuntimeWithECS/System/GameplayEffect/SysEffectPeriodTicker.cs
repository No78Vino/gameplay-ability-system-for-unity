using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysCheckEffectOngoingActive))]
    [UpdateBefore(typeof(SysInstantEffectModifyBaseValue))]
    public partial struct SysEffectPeriodTicker : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComPeriod>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            var currentFrame = globalFrameTimer.ValueRO.Frame;
            var currentTurn = globalFrameTimer.ValueRO.Turn;

            foreach (var (duration, inUsage, period, geEntity) in SystemAPI
                         .Query<RefRO<ComDuration>, RefRO<ComInUsage>, RefRW<ComPeriod>>()
                         .WithEntityAccess())
            {
                // 过滤不合法的GE
                if (!SystemAPI.IsComponentEnabled<ComInUsage>(geEntity)) continue;
                // 过滤未激活的GE
                if (!duration.ValueRO.active) continue;

                var time = duration.ValueRO.timeUnit == TimeUnit.Frame ? currentFrame : currentTurn;
                if (period.ValueRO.StartTime == 0) period.ValueRW.StartTime = time - 1 < 0 ? 0 : time;

                if (time - period.ValueRO.StartTime >= period.ValueRO.Period)
                {
                    period.ValueRW.StartTime = time;
                    foreach (var ge in period.ValueRO.GameplayEffects)
                    {
                        // 实例化GE
                        var instanceGe = GASManager.EntityManager.Instantiate(ge);
                        GasQueueCenter.AddEffectWaitingForApplication(instanceGe, inUsage.ValueRO.Target,
                            inUsage.ValueRO.Target);
                    }
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}