using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseApplicationEnd
{
    [UpdateInGroup(typeof(SysGroupApplicationEnd))]
    [UpdateAfter(typeof(SRemoveApplicationProgressTag))]
    public partial struct SDestroyEffects : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectDestroy>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            //
            // foreach (var (_,ge) in SystemAPI.Query<RefRO<CEffectDestroy>>().WithEntityAccess())
            // {
            //     ecb.DestroyEntity(ge);
            // }
            //
            // ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}