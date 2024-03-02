namespace GAS.Runtime.Ability.TimelineAbility.AbilityTask
{
    public abstract class OngoingAbilityTask:AbilityTaskBase
    {
#if UNITY_EDITOR
        public virtual void OnEditorPreviewTick(int frameIndex,int startFrame,int endFrame)
        {
        }
#endif
        public abstract OngoingAbilityTaskSpec CreateBaseSpec(AbilitySpec abilitySpec);
    }
    
    public abstract class OngoingAbilityTask<T> : OngoingAbilityTask where T:OngoingAbilityTaskSpec,new()
    {
        public T CreateSpec(AbilitySpec abilitySpec)
        {
            return this.CreateSpec<T>(abilitySpec);
        }
    }
    
    public abstract class OngoingAbilityTaskSpec:AbilityTaskSpec
    {
        public abstract void OnStart(int startFrame);

        public abstract void OnEnd(int endFrame);

        public abstract void OnTick(int frameIndex,int startFrame,int endFrame);
    }
}