using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateBefore(typeof(SysEffectTicker))]
    [UpdateAfter(typeof(SysCheckEffectOngoingActive))]
    public partial struct SysEffectDeactivate : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComNeedDeactivate>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            var currentFrame = globalFrameTimer.ValueRO.Frame;
            var currentTurn = globalFrameTimer.ValueRO.Turn;
            
            foreach (var (_,duration,ge) in SystemAPI.Query<RefRO<ComNeedDeactivate>,RefRW<ComDuration>>().WithEntityAccess())
            {
                // TODO 失活GE的对应逻辑
                // TODO 触发各种失活事件，回调
                // 1. 更新剩余时长时间
                if (duration.ValueRO.StopTickWhenDeactivated)
                {
                    var remainTime = duration.ValueRO.remianTime == 0
                        ? duration.ValueRO.duration
                        : duration.ValueRO.remianTime;

                    var time = duration.ValueRO.timeUnit == TimeUnit.Frame ? currentFrame : currentTurn;
                    duration.ValueRW.remianTime = remainTime - (time - duration.ValueRO.lastActiveTime);
                }
                
                // 2.触发失活 Cue
                // TriggerCueOnDeactivation();
                
                // 3.复原ASC的临时标签
                // Owner.GameplayTagAggregator.RestoreGameplayEffectDynamicTags(this);
                
                // 4.关闭Granted Ability
                // TryDeactivateGrantedAbilities();
                
                // 完成任务后删除执行标签
                ecb.RemoveComponent<ComNeedDeactivate>(ge);
            }
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}