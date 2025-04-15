using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateInGroup(typeof(SysGroupInstantEffect))]
    [UpdateAfter(typeof(STriggerCueOnExecution))]
    public partial struct SInstantEffectOver : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            // foreach (var (_, ge) in SystemAPI
            //              .Query<RefRW<CInUsage>>()
            //              .WithNone<CDuration>()
            //              .WithEntityAccess())
            // {
            //     ecb.AddComponent<CEffectDestroy>(ge);
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