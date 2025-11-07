using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup( typeof(SGDeactivateEffect))]
    public partial struct SDeactivateEnd : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipDeactivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<CDuration>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (_, _, inUsage, duration,ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipDeactivateEffect>,
                         RefRO<CEffectInUsage>,
                         RefRO<CDuration>>().WithEntityAccess())
            {
                // 结束停用阶段
                ecb.RemoveComponent<WipDeactivateEffect>(ge);
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