namespace GAS.Runtime.Ability.AbilityTask
{
    public abstract class AbstractAbilityTask
    {
        protected AbilitySpec _spec;
        public AbstractAbilityTask(AbilitySpec spec)
        {
            _spec = spec;
        }
        public abstract void Execute(params object[] args);
    }
}