using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGActivateEffect))]
    public partial struct SActivateEnd : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipActivateEffect>();
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
                         RefRO<WipActivateEffect>,
                         RefRO<CEffectInUsage>,
                         RefRO<CDuration>>().WithEntityAccess())
            {
                // 结束激活阶段
                ecb.RemoveComponent<WipActivateEffect>(ge);
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