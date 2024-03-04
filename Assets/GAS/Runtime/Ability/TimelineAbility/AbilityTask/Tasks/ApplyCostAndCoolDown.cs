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