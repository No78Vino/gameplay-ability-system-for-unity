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
            state.RequireForUpdate<WipInstantiateEffect>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (_,ge) in SystemAPI.Query<RefRO<WipInstantiateEffect>>().WithEntityAccess())
            {
                ecb.AddComponent<CEffectInstance>(ge);
                ecb.RemoveComponent<WipInstantiateEffect>(ge);
                
                // 进入下一阶段（Check Apply Effect）
                ecb.AddComponent<WipCheckApplyEffect>(ge);
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