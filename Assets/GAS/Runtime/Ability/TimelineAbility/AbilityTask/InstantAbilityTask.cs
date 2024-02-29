using System;
using Cysharp.Threading.Tasks;

namespace GAS.Runtime.Ability.TimelineAbility.AbilityTask
{
    [Serializable]
    public abstract class InstantAbilityTask:AbstractAbilityTask
    {
        public override void Init(AbilitySpec spec)
        {
            base.Init(spec);
        }
        
        public abstract void Execute();
    }
}