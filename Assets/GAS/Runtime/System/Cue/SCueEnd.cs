using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpDisplay))]
    [UpdateAfter(typeof(SCueTick))]
    public partial struct SCueEnd : ISystem
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
                     SystemAPI.Query<RefRO<ECCuePlaying>, MCCue>()
                         .WithDisabled<ECCuePlayable>()
                         .WithEntityAccess())
            {
                SystemAPI.SetComponentEnabled<ECCuePlaying>(cue, false);
                // 失活Cue
                mcCue.cue.OnDeactivate(Time.time);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}