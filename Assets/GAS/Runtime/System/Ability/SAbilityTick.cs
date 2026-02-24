using Unity.Burst;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGAbility))]
    [UpdateAfter(typeof(STryEndAbility))]
    public partial struct SAbilityTick : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CAbilityActive>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = EntityHelper.RegisterEntityCommandBuffer();
            
            var globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            
            foreach (var (_, abilityLogic) in SystemAPI.Query<RefRO<CAbilityActive>, MCAbilityLogic>())
            {
                abilityLogic.Logic.AbilityTick(globalTimer.ValueRO);
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