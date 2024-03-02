namespace GAS.Runtime.Ability.TimelineAbility.AbilityTask
{
    public abstract class InstantAbilityTask:AbilityTaskBase
    {
#if UNITY_EDITOR
        public virtual void OnEditorPreview()
        {
        }
#endif
        public abstract InstantAbilityTaskSpec CreateBaseSpec(AbilitySpec abilitySpec);
    }
    
    public abstract class InstantAbilityTask<T> : InstantAbilityTask where T:InstantAbilityTaskSpec,new()
    {
        public T CreateSpec(AbilitySpec abilitySpec)
        {
            return this.CreateSpec<T>(abilitySpec);
        }
    }
    
    public abstract class InstantAbilityTaskSpec:AbilityTaskSpec
    {
        public abstract void OnExecute();
    }
}