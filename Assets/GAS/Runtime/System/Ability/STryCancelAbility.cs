using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(SGAbility))]
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
            var ecb = EntityHelper.RegisterEntityCommandBuffer();
            var globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            
            foreach (var (_,ability) in SystemAPI.Query<RefRO<CAbilityInTryCancel>>().WithEntityAccess())
            {
                bool result = state.EntityManager.HasComponent<CAbilityActive>(ability);
                if (result)
                {
                    ecb.RemoveComponent<CAbilityActive>(ability);
                    ASCHelper.RestoreDynamicTags(ability);
                    var abilityLogic = state.EntityManager.GetComponentData<MCAbilityLogic>(ability);
                    abilityLogic.Logic.CancelAbility(globalTimer.ValueRO);
                    GASEventCenter.InvokeOnCancelAbility(ability);
                }
                ecb.RemoveComponent<CAbilityInTryCancel>(ability);
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