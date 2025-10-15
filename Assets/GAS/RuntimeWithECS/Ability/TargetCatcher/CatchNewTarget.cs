using System.Collections.Generic;

namespace GAS.Runtime
{
    public sealed class CatchNewTarget : NewTargetCatcherBase
    {
        protected override void CatchTargetsNonAlloc(AbilitySystemCellMono mainTarget, List<AbilitySystemCellMono> results)
        {
            results.Add(mainTarget);
        }
    }
}