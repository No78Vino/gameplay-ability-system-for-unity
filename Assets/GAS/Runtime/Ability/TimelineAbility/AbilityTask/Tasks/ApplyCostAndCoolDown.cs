using System;

namespace GAS.Runtime.Ability
{
    [Serializable]
    public class ApplyCostAndCoolDown:InstantAbilityTask
    {
        public float test;
        
        public override void OnExecute()
        {
            _spec.DoCost();
        }
    }
}