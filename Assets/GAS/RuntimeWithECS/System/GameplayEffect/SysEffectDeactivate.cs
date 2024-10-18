using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysEffectTicker))]
    [UpdateBefore(typeof(SysRemoveInvalidEffect))]
    public partial struct SysEffectDeactivate : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComNeedDeactivate>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (_,ge) in SystemAPI.Query<RefRO<ComNeedDeactivate>>().WithEntityAccess())
            {
                // TODO 失活GE的对应逻辑
                // TODO 触发各种失活事件，回调
                // TriggerCueOnDeactivation();
                // Owner.GameplayTagAggregator.RestoreGameplayEffectDynamicTags(this);
                // TryDeactivateGrantedAbilities();
                
                // 完成任务后删除执行标签
                ecb.RemoveComponent<ComNeedDeactivate>(ge);
            }
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}