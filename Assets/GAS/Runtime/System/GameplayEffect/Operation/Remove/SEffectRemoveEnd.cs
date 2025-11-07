using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGRemoveEffect))]
    public partial struct SEffectRemoveEnd : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipRemoveEffect>();
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
                         RefRO<WipRemoveEffect>,
                         RefRO<CEffectInUsage>,
                         RefRO<CDuration>>().WithEntityAccess())
            {
                // 结束移除阶段
                ecb.RemoveComponent<WipRemoveEffect>(ge);
                
                // 移除效果实例标签，并进入销毁阶段
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