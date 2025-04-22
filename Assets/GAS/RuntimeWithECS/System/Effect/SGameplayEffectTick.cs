using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.Runtime;
using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGroupTickGameplayEffect))]
    public partial struct SGameplayEffectTick : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CDuration>();
            state.RequireForUpdate<CEffectInUsage>();
            state.RequireForUpdate<CEffectApplied>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}