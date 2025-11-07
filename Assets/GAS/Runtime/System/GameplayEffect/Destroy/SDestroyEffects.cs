using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGEffectDestroy))]
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
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (_,ge) in SystemAPI.Query<RefRO<CEffectDestroy>>().WithEntityAccess())
            {
                ecb.DestroyEntity(ge);
                ecb.RemoveComponent<CEffectDestroy>(ge);
            }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}