using GAS.Runtime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SysGrpTickAbility))]
    public partial struct SAbilityTick : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CAbilityActive>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            EntityHelper.RegisterEntityCommandBuffer(ecb);
            
            var globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            
            foreach (var (_, abilityLogic) in SystemAPI.Query<RefRO<CAbilityActive>, MCAbilityLogic>())
            {
                abilityLogic.Logic.AbilityTick(globalTimer.ValueRO);
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