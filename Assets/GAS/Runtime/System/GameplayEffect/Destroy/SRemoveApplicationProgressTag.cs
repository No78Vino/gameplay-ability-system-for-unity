using GAS.Runtime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime.System.GameplayEffect.PhaseApplicationEnd
{
    [UpdateInGroup(typeof(SysGrpKillEffect))]
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