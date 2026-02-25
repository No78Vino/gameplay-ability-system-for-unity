using System.Collections.Generic;

namespace GAS.Runtime
{
    public sealed class CatchSelf : TargetCatcherBase<XParamNone>  
    {  
        protected override void CatchTargetsNonAlloc(AbilitySystemCell mainTarget, List<AbilitySystemCell> results)  
        {  
            results.Add(Owner);  
        }  
    }
}