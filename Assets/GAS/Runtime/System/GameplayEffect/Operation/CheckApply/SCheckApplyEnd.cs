using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    /// <summary>
    ///  检查施加效果 结束
    /// </summary>
    [UpdateInGroup(typeof(SGCheckApplyEffect))]
    public partial struct SCheckApplyEnd : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipCheckApplyEffect>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, ge) in SystemAPI.Query<RefRO<WipCheckApplyEffect>>().WithEntityAccess())
            {
                // 完成检查，移除标记组件
                ecb.RemoveComponent<WipCheckApplyEffect>(ge);
                
                // 筛选可以进入下一阶段（Apply Effect）的效果实例
                if (SystemAPI.HasComponent<CEffectInstance>(ge))
                    ecb.AddComponent<WipApplyEffect>(ge);
                
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