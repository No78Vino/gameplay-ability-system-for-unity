using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGAbility))]
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
            var ecb = EntityHelper.RegisterEntityCommandBuffer();
            var globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            
            foreach (var (_, baseInfo, ability) in SystemAPI.Query<RefRO<CAbilityInTryEnd>, RefRO<CAbilityBaseInfo>>()
                         .WithEntityAccess())
            {
                var result = state.EntityManager.HasComponent<CAbilityActive>(ability);
                if (result)
                {
                    ecb.RemoveComponent<CAbilityActive>(ability);
                    ASCHelper.RestoreDynamicTags(ability);
                    var abilityLogic = state.EntityManager.GetComponentData<MCAbilityLogic>(ability);
                    abilityLogic.Logic.EndAbility(globalTimer.ValueRO);
                    GASEventCenter.InvokeOnEndAbility(ability);
                }

                ecb.RemoveComponent<CAbilityInTryEnd>(ability);
            }

            ecb.Playback(state.EntityManager);
            EntityHelper.ExecuteDeferredCommands();
            ecb.Dispose();
            EntityHelper.UnregisterEntityCommandBuffer();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}