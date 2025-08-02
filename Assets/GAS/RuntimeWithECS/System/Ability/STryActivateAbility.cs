using GAS.Runtime;
using GAS.RuntimeWithECS;
using GAS.RuntimeWithECS.Dynamic;
using GAS.RuntimeWithECS.Static;
using GAS.RuntimeWithECS.Tag;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
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
            EntityHelper.RegisterEntityCommandBuffer(ecb);
            
            var globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            
            foreach (var (_, basicInfo, ability) in SystemAPI
                         .Query<RefRO<CAbilityInTryActivate>, RefRO<CAbilityBaseInfo>>().WithEntityAccess())
            {
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
                    abilityLogic.Logic.ActivateAbility(globalTimer.ValueRO);
                }

                GASEventCenter.InvokeOnActivateResult(ability, result);

                ecb.RemoveComponent<CAbilityInTryActivate>(ability);
            }

            ecb.Playback(state.EntityManager);
            EntityHelper.UnregisterEntityCommandBuffer();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}