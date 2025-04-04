using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Core;
using GAS.RuntimeWithECS.System.SystemGroup.LogicTick;
using Unity.Burst;
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
            var globalTimer = SystemAPI.GetSingletonRW<GlobalTimer>();
            foreach (var (_, abilityLogic) in SystemAPI.Query<RefRO<CAbilityActive>, MCAbilityLogic>())
                abilityLogic.Logic.AbilityTick(globalTimer.ValueRO);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}