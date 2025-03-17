using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.AbilitySystemCell;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.System.SystemGroup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.Ability.PhaseActivation
{
    [UpdateInGroup(typeof(SysGroupAbility))]
    [UpdateAfter(typeof(STryCancelAbility))]
    public partial struct STryEndAbility : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CAbilityInTryEnd>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (_,ability) in SystemAPI.Query<RefRO<CAbilityInTryEnd>>().WithEntityAccess())
            {
                bool result = state.EntityManager.HasComponent<CAbilityActive>(ability);
                if (result)
                {
                    ecb.RemoveComponent<CAbilityActive>(ability);
                    ASCUtil.RestoreDynamicTags(ability);
                    var abilityLogic = state.EntityManager.GetComponentData<MCAbilityLogic>(ability);
                    abilityLogic.Logic.EndAbility();
                    GASEventCenter.InvokeOnEndAbility(ability);
                }
                ecb.RemoveComponent<CAbilityInTryEnd>(ability);
            }
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}