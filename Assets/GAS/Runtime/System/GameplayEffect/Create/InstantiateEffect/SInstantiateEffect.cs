using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGInstantiateEffect))]
    public partial struct SInstantiateEffect : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CTagWaitingInstantiateEffect>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (_,ge) in SystemAPI.Query<RefRO<CTagWaitingInstantiateEffect>>().WithEntityAccess())
            {
                ecb.AddComponent<CEffectInstance>(ge);
                ecb.RemoveComponent<CTagWaitingInstantiateEffect>(ge);
            }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}