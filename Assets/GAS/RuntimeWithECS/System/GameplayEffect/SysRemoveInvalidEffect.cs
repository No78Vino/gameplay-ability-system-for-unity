using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysCheckEffectApplicationValid))]
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

            foreach (var geBuff in SystemAPI.Query<DynamicBuffer<GameplayEffectBufferElement>>())
                for (var i = geBuff.Length - 1; i >= 0; i--)
                {
                    var ge = geBuff[i].GameplayEffect;
                    if (SystemAPI.IsComponentEnabled<ComInUsage>(ge)) continue;
                    geBuff.RemoveAt(i);
                    // 含有子实例的组件也要清理
                    if (SystemAPI.HasComponent<ComPeriod>(ge))
                    {
                        var period = SystemAPI.GetComponentRO<ComPeriod>(ge);
                        foreach (var sonGe in period.ValueRO.GameplayEffects)
                            ecb.DestroyEntity(sonGe);
                    }
                    ecb.DestroyEntity(ge);
                }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}