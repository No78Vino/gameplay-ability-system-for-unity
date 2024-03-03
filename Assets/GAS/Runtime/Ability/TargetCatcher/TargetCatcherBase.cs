using System.Collections.Generic;
using GAS.Runtime.Component;
using UnityEngine.UIElements;

namespace GAS.Runtime.Ability.TargetCatcher
{
    public abstract class TargetCatcherBase
    {
        public AbilitySystemComponent Owner;

        public TargetCatcherBase()
        {
            
        }
        
        public virtual void Init(AbilitySystemComponent owner)
        {
            Owner = owner;
        }
        
        public abstract List<AbilitySystemComponent> CatchTargets(AbilitySystemComponent mainTarget);


    }
}