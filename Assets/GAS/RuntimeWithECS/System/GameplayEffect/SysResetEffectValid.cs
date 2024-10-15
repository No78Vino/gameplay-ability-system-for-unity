using GAS.RuntimeWithECS.GameplayEffect.Component;
using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect
{
    [UpdateAfter(typeof(SysApplyEffect))]
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
            foreach (var (vBasicInfo,_) in SystemAPI.Query<RefRW<ComBasicInfo>,RefRO<ComInUsage>>())
            {
                vBasicInfo.ValueRW.Valid = true;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}