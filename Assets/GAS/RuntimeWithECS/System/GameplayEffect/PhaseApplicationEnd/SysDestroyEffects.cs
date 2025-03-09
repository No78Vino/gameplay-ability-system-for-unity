using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.Common.Component;
using GAS.RuntimeWithECS.GameplayEffect.Component;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.GameplayEffect.PhaseApplicationEnd
{
    [UpdateInGroup(typeof(SysGroupApplicationEnd))]
    [UpdateAfter(typeof(SysRemoveApplicationProgressTag))]
    public partial struct SysDestroyEffects : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CDestroy>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (_,ge) in SystemAPI.Query<RefRO<CDestroy>>().WithEntityAccess())
            {
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