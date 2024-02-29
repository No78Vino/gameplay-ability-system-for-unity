using System;

namespace GAS.Runtime.Ability.TimelineAbility.AbilityTask
{
    [Serializable]
    public abstract class AbstractAbilityTask
    {
        protected AbilitySpec _spec;
        public virtual void Init(AbilitySpec spec)
        {
            _spec = spec;
        }
    }
}