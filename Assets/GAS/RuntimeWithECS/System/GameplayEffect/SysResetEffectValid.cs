using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    public partial struct SysResetEffectValid : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ComInUsage>();
            state.RequireForUpdate<ComBasicInfo>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}