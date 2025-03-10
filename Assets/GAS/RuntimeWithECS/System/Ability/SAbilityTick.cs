using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.Ability.Component.Static;
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
            foreach (var (_,abilityLogic) in SystemAPI.Query<RefRO<CAbilityActive>,MCAbilityLogic>())
            {
                abilityLogic.Logic.AbilityTick();
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}