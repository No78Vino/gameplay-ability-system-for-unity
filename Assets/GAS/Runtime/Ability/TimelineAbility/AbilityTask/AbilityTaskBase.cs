namespace GAS.Runtime.Ability
{
    public abstract class AbilityTaskBase
    {
        protected AbilitySpec _spec;
        public virtual void Init(AbilitySpec spec)
        {
            _spec = spec;
        }
    }
}