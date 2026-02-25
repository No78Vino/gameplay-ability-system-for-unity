using System.Collections.Generic;

namespace GAS.Runtime
{
    public sealed class CatchTarget : TargetCatcherBase<XParamNone>  
    {  
        protected override void CatchTargetsNonAlloc(AbilitySystemCell mainTarget, List<AbilitySystemCell> results)  
        {  
            results.Add(mainTarget);  
        }  
    }
}