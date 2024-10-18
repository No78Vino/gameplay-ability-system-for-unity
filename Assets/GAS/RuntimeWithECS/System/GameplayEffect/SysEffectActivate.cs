using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysEffectTicker))]
    [UpdateBefore(typeof(SysRemoveInvalidEffect))]
    public partial struct SysEffectActivate : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComNeedActivate>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (_,ge) in SystemAPI.Query<RefRO<ComNeedActivate>>().WithEntityAccess())
            {
                // TODO 激活GE的对应逻辑
                // TODO 触发各种激活事件，回调
                // ActivationTime = Time.time;
                // TriggerOnActivation();
                
                // 完成任务后删除执行标签
                ecb.RemoveComponent<ComNeedActivate>(ge);
            }
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}