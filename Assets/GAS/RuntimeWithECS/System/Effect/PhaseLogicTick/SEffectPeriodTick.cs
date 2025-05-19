using GAS.Runtime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGroupTickGameplayEffect))]
    public partial struct SEffectPeriodTick : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GlobalTimer>();
            state.RequireForUpdate<CPeriod>();
            state.RequireForUpdate<CEffectApplied>();
            state.RequireForUpdate<CDuration>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            var currentFrame = globalFrameTimer.ValueRO.Frame;
            var currentTurn = globalFrameTimer.ValueRO.Turn;
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (duration, inUsage, period,_, geEntity) in SystemAPI
                         .Query<RefRO<CDuration>, RefRO<CEffectInUsage>, RefRW<CPeriod>,RefRO<CEffectApplied>>()
                         .WithNone<CInApplicationProgress>()
                         .WithEntityAccess())
            {
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
                        var instanceGe = state.EntityManager.Instantiate(ge);
                        ecb.AddComponent<CEffectApplied>(instanceGe);
                        ecb.AddComponent<CInApplicationProgress>(instanceGe);
                        ecb.AddComponent<CEffectInUsage>(instanceGe);
                        ecb.SetComponent(instanceGe, new CEffectInUsage()
                        {
                            Level = inUsage.ValueRO.Level,
                            Target = inUsage.ValueRO.Target,
                            Source = inUsage.ValueRO.Source,
                        });
                    }
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