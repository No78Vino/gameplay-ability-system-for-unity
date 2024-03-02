using GAS.Runtime.Ability.TimelineAbility.AbilityTask;

namespace GAS.Runtime.Ability
{
    public class ApplyCostAndCoolDown:InstantAbilityTask<ApplyCostAndCoolDownSpec>
    {
        public override InstantAbilityTaskSpec CreateBaseSpec(AbilitySpec abilitySpec)
        {
            return CreateSpec(abilitySpec);
        }
    }
    
    public class ApplyCostAndCoolDownSpec:InstantAbilityTaskSpec
    {
        public override void OnExecute()
        {
            _spec.DoCost();
        }
    }
}