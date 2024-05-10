using System.Collections.Generic;

namespace GAS.Runtime
{
    public sealed class CatchTarget : TargetCatcherBase
    {
        protected override void CatchTargetsNonAlloc(AbilitySystemComponent mainTarget, List<AbilitySystemComponent> results)
        {
            results.Add(mainTarget);
        }
    }
}