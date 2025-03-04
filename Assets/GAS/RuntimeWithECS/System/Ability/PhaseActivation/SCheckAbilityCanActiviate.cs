using Unity.Burst;
using Unity.Entities;

namespace GAS.RuntimeWithECS.System.Ability.PhaseActivation
{
    public partial struct SCheckAbilityCanActiviate : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // _abilityArguments = args;
            // var result = CanActivate();
            // var success = result == AbilityActivateResult.Success;
            // if (success)
            // {
            //     IsActive = true;
            //     ActiveCount++;
            //     Owner.GameplayTagAggregator.ApplyGameplayAbilityDynamicTag(this);
            //
            //     ActivateAbility(_abilityArguments);
            // }
            //
            // _onActivateResult?.Invoke(result);
            // return success;
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}