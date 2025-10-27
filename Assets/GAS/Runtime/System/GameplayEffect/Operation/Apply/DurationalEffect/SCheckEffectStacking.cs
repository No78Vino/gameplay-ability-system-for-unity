using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGDurationalEffect))]
    public partial struct SCheckEffectStacking : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<WipApplyEffect>();
            state.RequireForUpdate<CStacking>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // TODO: 实现效果堆叠检查逻辑
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}