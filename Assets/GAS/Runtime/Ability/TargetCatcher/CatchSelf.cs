using System.Collections.Generic;

namespace GAS.Runtime
{
    public sealed class CatchSelf : TargetCatcherBase
    {
        public override void CatchTargetsNonAlloc(AbilitySystemComponent mainTarget, List<AbilitySystemComponent> results)
        {
            results.Clear();
            results.Add(Owner);
        }
    }
}