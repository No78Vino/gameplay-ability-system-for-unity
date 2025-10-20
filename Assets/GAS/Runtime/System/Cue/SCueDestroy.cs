using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpDisplay))]
    [UpdateAfter(typeof(SCueEnd))]
    public partial struct SCueDestroy : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ECKillCue>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            foreach (var (_,mcCue,cueEntity) in SystemAPI.Query<RefRO<ECKillCue>,MCCue>().WithEntityAccess())
            {
                // 触发销毁时回调
                mcCue.cue.OnDestroy(Time.time);
                // 销毁Cue
                ecb.DestroyEntity(cueEntity);
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