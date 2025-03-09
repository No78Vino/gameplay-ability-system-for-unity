using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.Ability.PhaseActivation
{
    [UpdateInGroup(typeof(SysGroupAbility))]
    [UpdateAfter(typeof(STryActivateAbility))]
    public partial struct STryCancelAbility : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CAbilityInTryCancel>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (_,ability) in SystemAPI.Query<RefRO<CAbilityInTryCancel>>().WithEntityAccess())
            {
                GAUtil.TryCancelAbility(ability);
                ecb.RemoveComponent<CAbilityInTryCancel>(ability);
            }
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}