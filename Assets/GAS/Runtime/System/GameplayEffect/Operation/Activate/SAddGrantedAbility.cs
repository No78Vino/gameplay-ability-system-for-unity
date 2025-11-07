using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGActivateEffect))]
    [UpdateBefore(typeof(SActivateEnd))]
    public partial struct SAddGrantedAbility : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipActivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<MCGrantedAbility>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // TODO 添加Granted Ability
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}