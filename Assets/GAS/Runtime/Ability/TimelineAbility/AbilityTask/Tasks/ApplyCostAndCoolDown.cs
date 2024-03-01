using GAS.Runtime.Ability.TimelineAbility.AbilityTask;

namespace GAS.Runtime.Ability
{
    public class ApplyCostAndCoolDown:InstantAbilityTask
    {
        public override void OnExecute()
        {
            _spec.DoCost();
        }
    }
}