using System.Collections.Generic;

namespace GAS.Runtime
{
    public sealed class CatchNewTarget : NewTargetCatcherBase
    {
        protected override void CatchTargetsNonAlloc(AbilitySystemComponent mainTarget, List<AbilitySystemComponent> results)
        {
            results.Add(mainTarget);
        }
    }
}