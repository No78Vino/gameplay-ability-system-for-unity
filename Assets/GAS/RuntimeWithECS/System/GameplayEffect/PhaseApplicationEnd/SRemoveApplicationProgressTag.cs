using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseApplicationEnd
{
    [UpdateInGroup(typeof(SysGroupApplicationEnd))]
    public partial struct SRemoveApplicationProgressTag : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CInApplicationProgress>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            //
            // foreach (var (_,ge) in SystemAPI.Query<RefRO<CInApplicationProgress>>().WithEntityAccess())
            // {
            //     ecb.RemoveComponent<CInApplicationProgress>(ge);
            //     if(SystemAPI.HasComponent<CValidEffect>(ge))
            //         ecb.RemoveComponent<CValidEffect>(ge);
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