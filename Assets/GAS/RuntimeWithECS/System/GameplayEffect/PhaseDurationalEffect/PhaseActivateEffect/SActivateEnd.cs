using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseDurationalEffect
{
    [UpdateInGroup(typeof(SysGroupActivateEffect))]
    public partial struct SActivateEnd : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CInActivationProgress>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            // foreach (var (_, ge) in SystemAPI.Query<RefRO<CInActivationProgress>>().WithEntityAccess())
            //     ecb.RemoveComponent<CInActivationProgress>(ge);
            //
            // ecb.Playback(state.EntityManager);
            // ecb.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}