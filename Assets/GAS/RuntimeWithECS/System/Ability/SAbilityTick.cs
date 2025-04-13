using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.System.SystemGroup.LogicTick;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.Ability
{
    [UpdateInGroup(typeof(SysGroupTickAbility))]
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
            var globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            foreach (var (_, abilityLogic) in SystemAPI.Query<RefRO<CAbilityActive>, MCAbilityLogic>())
            {
                abilityLogic.Logic.UpdateEntityCommandBuffer(ecb);
                abilityLogic.Logic.AbilityTick(globalTimer.ValueRO);
                abilityLogic.Logic.RemoveEntityCommandBuffer();
            }
            
            ecb.Playback(state.EntityManager);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}