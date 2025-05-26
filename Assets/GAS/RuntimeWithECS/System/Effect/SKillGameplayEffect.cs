using GAS.RuntimeWithECS.Common.Component;
using GAS.Runtime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGroupEffect))]
    [UpdateAfter(typeof(SApplyGameplayEffect))]
    public partial struct SKillGameplayEffect : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<CEffectDestroy>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, ge) in SystemAPI.Query<RefRO<CEffectDestroy>>()
                         .WithEntityAccess())
            {
                // 1.如果是持续型buff， 先尝试从asc的ge容器中移除
                if (SystemAPI.HasComponent<CDuration>(ge) && SystemAPI.HasComponent<CEffectInUsage>(ge))
                {
                    var inUsage = SystemAPI.GetComponentRO<CEffectInUsage>(ge);
                    var targetAsc = inUsage.ValueRO.Target;
                    var geContainer = SystemAPI.GetBuffer<BEGameplayEffect>(targetAsc);
                    for (var i = 0; i < geContainer.Length; i++)
                    {
                        if (geContainer[i].GameplayEffect != ge) continue;
                        geContainer.RemoveAt(i);
                        break;
                    }
                }

                // 2.销毁绑定的entity：各类cue组件，派生ge等等
                // TODO
                
                // 3.销毁ge
                ecb.DestroyEntity(ge);
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