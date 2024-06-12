using System;

namespace GAS.Runtime
{
    [Serializable]
    public class ApplyCostAndCoolDown : InstantAbilityTask
    {
        public override void OnExecute()
        {
            _spec.DoCost();
        }
    }
}