using GAS.Runtime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGroupActivateEffect))]
    [UpdateBefore(typeof(SActivateEnd))]
    public partial struct SActivateEffect : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CDuration>();
            state.RequireForUpdate<CInApplicationProgress>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            //
            // var globalFrameTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            // //  更新激活时间
            // foreach (var (_, duration) in SystemAPI.Query<RefRO<CInActivationProgress>, RefRW<CDuration>>())
            // {
            //     UpdateActiveTime(ref duration.ValueRW,globalFrameTimer.ValueRO);
            // }
            //
            // ecb.Playback(state.EntityManager);
            // ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
        
        public static void UpdateActiveTime(ref CDuration duration, GlobalTimer globalFrameTimer)
        {
            var currentFrame = globalFrameTimer.Frame;
            var currentTurn = globalFrameTimer.Turn;
            //  更新激活时间
            if (duration.active) return;
            duration.active = true;
            if (duration.timeUnit == TimeUnit.Frame)
            {
                if (duration.activeTime == 0 || duration.ResetStartTimeWhenActivated)
                    duration.activeTime = currentFrame;

                duration.lastActiveTime = currentFrame;
            }
            else
            {
                if (duration.activeTime == 0 || duration.ResetStartTimeWhenActivated)
                    duration.activeTime = currentTurn;

                duration.lastActiveTime = currentTurn;
            }
        }
    }
}