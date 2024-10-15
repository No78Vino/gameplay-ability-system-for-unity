using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    public partial struct SysRemoveInvalidEffect : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameplayEffectBufferElement>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            // foreach (var (vBasicInfo, _,entity) in SystemAPI
            //              .Query<RefRO<GameplayEffectBufferElement>,  RefRO<ComInUsage>>().WithEntityAccess())
            // {
            //     bool isValid = vBasicInfo.ValueRO.Valid;
            //     if (!isValid)
            //     {
            //         // TODO 移除不合法GE
            //         // 移除【在使用中】的标签
            //         ecb.RemoveComponent<ComInUsage>(entity);
            //
            //         var asc = vBasicInfo.ValueRO.Target;
            //         var effects = SystemAPI.GetBuffer<GameplayEffectBufferElement>(asc);
            //         effects.Remove();
            //     }
            // }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}