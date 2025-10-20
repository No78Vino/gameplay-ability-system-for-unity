using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SysGrpDisplay))]
    [UpdateAfter(typeof(SCueStart))]
    public partial struct SCueTick : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ECCuePlaying>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, mcCue) in SystemAPI.Query<RefRO<ECCuePlaying>, MCCue>()) 
                mcCue.cue.OnTick(Time.time);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}