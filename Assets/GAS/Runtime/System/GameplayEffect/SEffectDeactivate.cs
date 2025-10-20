using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime.System.GameplayEffect
{
    //[UpdateBefore(typeof(SysEffectDurationTicker))]
    //[UpdateAfter(typeof(SysCheckEffectOngoingActive))]
    public partial struct SEffectDeactivate : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CInDeactivationProgress>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            //
            // var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            // var currentFrame = globalFrameTimer.ValueRO.Frame;
            // var currentTurn = globalFrameTimer.ValueRO.Turn;
            //
            // foreach (var (_,duration,ge) in SystemAPI.Query<RefRO<ComNeedDeactivate>,RefRW<ComDuration>>().WithEntityAccess())
            // {
            //     // TODO 失活GE的对应逻辑
            //     // TODO 触发各种失活事件，回调
            //     // 1. 更新剩余时长时间
            //     if (duration.ValueRO.StopTickWhenDeactivated)
            //     {
            //         var remainTime = duration.ValueRO.remianTime == 0
            //             ? duration.ValueRO.duration
            //             : duration.ValueRO.remianTime;
            //
            //         var time = duration.ValueRO.timeUnit == TimeUnit.Frame ? currentFrame : currentTurn;
            //         duration.ValueRW.remianTime = remainTime - (time - duration.ValueRO.lastActiveTime);
            //     }
            //     
            //     // 2.触发失活 Cue
            //     // TriggerCueOnDeactivation();
            //     
            //     // 3.复原ASC的临时标签
            //     // Owner.GameplayTagAggregator.RestoreGameplayEffectDynamicTags(this);
            //     
            //     // 4.关闭Granted Ability
            //     // TryDeactivateGrantedAbilities();
            //     
            //     // 5. 标记所有受影响的Attribute为dirty
            //     var inUsage = SystemAPI.GetComponentRO<ComInUsage>(ge);
            //     var asc = inUsage.ValueRO.Target;
            //     var isDirty = GameplayEffectUtils.CheckAscAttributeDirty(
            //         SystemAPI.GetBuffer<AttributeSetBufferElement>(asc),
            //         SystemAPI.GetBuffer<BuffEleModifier>(ge));
            //     if (isDirty) ecb.AddComponent<AttributeIsDirty>(asc);
            //     
            //     // 完成任务后删除执行标签
            //     ecb.RemoveComponent<ComNeedDeactivate>(ge);
            // }
            // ecb.Playback(state.EntityManager);
            // ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}