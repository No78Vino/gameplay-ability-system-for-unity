using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysTriggerInstantEffectModify))]
    public partial struct SysRemoveFinishEffect : ISystem
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

            foreach (var (geBuff,asc) in SystemAPI.Query<DynamicBuffer<GameplayEffectBufferElement>>().WithEntityAccess())
            {
                for (int i = geBuff.Length - 1; i >= 0; i--)
                {
                    var e = geBuff[i].GameplayEffect;
                    var isInstant = !SystemAPI.HasComponent<ComDuration>(e);
                    if (isInstant)
                    {
                        ecb.RemoveComponent<ComInUsage>(e);
                        geBuff.RemoveAt(i);
                    }
                }

                //ecb.SetBuffer<GameplayEffectBufferElement>(asc);
            }

            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}