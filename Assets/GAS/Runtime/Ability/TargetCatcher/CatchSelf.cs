using System.Collections.Generic;

namespace GAS.Runtime
{
    public sealed class CatchSelf : TargetCatcherBase
    {
        protected override void CatchTargetsNonAlloc(AbilitySystemCellMono mainTarget, List<AbilitySystemCellMono> results)
        {
            results.Add(Owner);
        }
    }
}