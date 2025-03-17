using GAS.RuntimeWithECS.Ability;
using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.System.SystemGroup;
using GAS.RuntimeWithECS.Tag;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.Ability.PhaseActivation
{
    [UpdateInGroup(typeof(SysGroupAbility))]
    public partial struct STryActivateAbility : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CAbilityInTryActivate>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (_,basicInfo,ability) in SystemAPI.Query<RefRO<CAbilityInTryActivate>,RefRO<CAbilityBaseInfo>>().WithEntityAccess())
            {
                //GAUtil.TryActivateAbility(ability);
                var result = GAUtil.CanActivateAbility(ability);
                if (result == AbilityActivationResult.Success)
                {
                    var owner = basicInfo.ValueRO.Owner;
                    if (state.EntityManager.HasComponent<CAbilityActivationOwnedTags>(ability))
                    {
                        var abilityActivationOwnedTags =
                            state.EntityManager.GetComponentData<CAbilityActivationOwnedTags>(ability);
                        foreach (var tag in abilityActivationOwnedTags.tags)
                            GTagUtil.AddTemporaryTagTo(owner, ability, tag);
                    }

                    // 添加激活tag
                    ecb.AddComponent(ability, new CAbilityActive());
                    // 激活能力【自定义逻辑】
                    var abilityLogic = state.EntityManager.GetComponentData<MCAbilityLogic>(ability);
                    abilityLogic.Logic.ActivateAbility();
                }
                GASEventCenter.InvokeOnActivateResult(ability, result);
                
                ecb.RemoveComponent<CAbilityInTryActivate>(ability);
            }
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}