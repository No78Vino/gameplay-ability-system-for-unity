using System.Collections.Generic;

namespace GAS.Runtime
{
    public sealed class NewCatchSelf : NewTargetCatcherBase
    {
        protected override void CatchTargetsNonAlloc(AbilitySystemComponent mainTarget, List<AbilitySystemComponent> results)
        {
            results.Add(Owner);
        }
    }
}