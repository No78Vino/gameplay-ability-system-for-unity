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

            foreach (var geBuff in SystemAPI.Query<DynamicBuffer<GameplayEffectBufferElement>>())
            {
                for (int i = geBuff.Length - 1; i >= 0; i--)
                {
                    var basicData = SystemAPI.GetComponentRO<ComBasicInfo>(geBuff[i].GameplayEffect);
                    if (basicData.ValueRO.Valid) continue;
                    ecb.RemoveComponent<ComInUsage>(geBuff[i].GameplayEffect);
                    geBuff.RemoveAt(i);
                }
            }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}