using System;

namespace GAS.Runtime.Ability
{
    [Serializable]
    public class ApplyCostAndCoolDown:InstantAbilityTask
    {
        public override void OnExecute()
        {
            _spec.DoCost();
        }
    }
}