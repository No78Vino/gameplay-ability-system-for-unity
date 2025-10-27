using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGInstantEffect))]
    [UpdateAfter(typeof(SExecuteInstantEffectModifiers))]
    public partial struct SExecuteInstantEffectEnd : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (_,ge) in SystemAPI
                         .Query<RefRO<CEffectInstance>>()
                         .WithNone<CDuration>()
                         .WithEntityAccess())
            {
                ecb.RemoveComponent<CEffectInstance>(ge);
                ecb.AddComponent<CEffectDestroy>(ge);
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}