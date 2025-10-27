using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGApplyEffect))]
    [UpdateAfter(typeof(SGInstantEffect))]
    [UpdateAfter(typeof(SGDurationalEffect))]
    public partial struct SApplyEnd : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipApplyEffect>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, ge) in SystemAPI.Query<RefRO<WipApplyEffect>>().WithEntityAccess())
            {
                // 完成应用，移除标记组件
                ecb.RemoveComponent<WipApplyEffect>(ge);
                
                // 过滤进入下一阶段（Check Active）的effect: Durational且还是实例状态的effect
                if(SystemAPI.HasComponent<CDuration>(ge) && SystemAPI.HasComponent<CEffectInstance>(ge))
                    ecb.AddComponent<WipCheckActiveEffect>(ge);
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