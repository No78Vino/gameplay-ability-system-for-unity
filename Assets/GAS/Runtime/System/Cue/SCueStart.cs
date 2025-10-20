using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpDisplay))]
    public partial struct SCueStart : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ECCuePlayable>();
            state.RequireForUpdate<ECCuePlaying>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, mcCue, cue) in
                     SystemAPI.Query<RefRO<ECCuePlayable>, MCCue>()
                         .WithDisabled<ECCuePlaying>()
                         .WithEntityAccess())
            {
                SystemAPI.SetComponentEnabled<ECCuePlaying>(cue, true);
                // 激活Cue
                mcCue.cue.OnActivate(Time.time);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}