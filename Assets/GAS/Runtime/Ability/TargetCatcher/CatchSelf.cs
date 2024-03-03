using System.Collections.Generic;
using GAS.Runtime.Component;
using UnityEngine.UIElements;

namespace GAS.Runtime.Ability.TargetCatcher
{
    public class CatchSelf : TargetCatcherBase
    {
        public override List<AbilitySystemComponent> CatchTargets(AbilitySystemComponent mainTarget)
        {
            return new List<AbilitySystemComponent> { Owner };
        }
    }
}