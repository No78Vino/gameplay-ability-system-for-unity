using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    /// <summary>
    /// 所有GE需要初始化的Runtime数据，组件等，都在这个系统内完成
    /// </summary>
    public partial struct SysInitEffect : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComNeedInit>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (_,ge) in SystemAPI.Query<RefRO<ComNeedInit>>().WithEntityAccess())
            {
                ecb.RemoveComponent<ComNeedInit>(ge);
            }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}