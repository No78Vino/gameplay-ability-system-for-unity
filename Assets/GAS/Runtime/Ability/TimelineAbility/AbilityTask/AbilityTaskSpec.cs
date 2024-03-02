namespace GAS.Runtime.Ability.TimelineAbility.AbilityTask
{
    public abstract class AbilityTaskSpec
    {
        AbilityTaskBase _taskAsset;
        protected AbilitySpec _spec;
        
        public virtual void Init(AbilityTaskBase taskAsset,AbilitySpec spec)
        {
            _taskAsset = taskAsset;
            _spec = spec;
        }
    }
}