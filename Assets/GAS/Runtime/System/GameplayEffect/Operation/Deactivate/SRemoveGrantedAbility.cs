using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGDeactivateEffect))]
    [UpdateBefore(typeof(SDeactivateEnd))]
    public partial struct SRemoveGrantedAbility : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipDeactivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<MCGrantedAbility>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // TODO 移除Granted Ability
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}