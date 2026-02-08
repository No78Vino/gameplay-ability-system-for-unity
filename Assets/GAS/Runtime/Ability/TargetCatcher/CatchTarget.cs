using System.Collections.Generic;

namespace GAS.Runtime
{
    public sealed class CatchTarget : TargetCatcherBase
    {
        protected override void CatchTargetsNonAlloc(AbilitySystemCellMono mainTarget, List<AbilitySystemCellMono> results)
        {
            results.Add(mainTarget);
        }
    }
}