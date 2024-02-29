using System;

namespace GAS.Runtime.Ability.TimelineAbility.AbilityTask
{
    [Serializable]
    public abstract class OngoingAbilityTask:AbstractAbilityTask
    {
        public override void Init(AbilitySpec spec)
        {
            base.Init(spec);
        }

        public abstract void OnStart();

        public abstract void OnEnd();

        public abstract void OnTick();
    }
}