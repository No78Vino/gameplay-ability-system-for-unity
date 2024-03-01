namespace GAS.Runtime.Ability.TimelineAbility.AbilityTask
{
    public abstract class OngoingAbilityTask:AbilityTaskBase
    {
        public abstract void OnStart();

        public abstract void OnEnd();

        public abstract void OnTick();
        
#if UNITY_EDITOR
        public virtual void OnEditorPreviewTick(int frameIndex,int startFrame,int endFrame)
        {
        }
#endif
    }
}