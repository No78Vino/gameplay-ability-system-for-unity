using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGrpTryApplyEffect))]
    public partial struct SCheckApplicationCondition : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<CEffectApplied>();
            state.RequireForUpdate<CApplicationCondition>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // TODO 校验ApplicationCondition
            // foreach (var (_, duration, ge) in SystemAPI.Query<RefRO<ComInUsage>, RefRW<ComValidEffect>>()
            //              .WithEntityAccess())
            // {
            // }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}