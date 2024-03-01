namespace GAS.Runtime.Ability.TimelineAbility.AbilityTask
{
    public abstract class OngoingAbilityTask:AbilityTaskBase
    {
        public abstract void OnStart(int startFrame);

        public abstract void OnEnd(int endFrame);

        public abstract void OnTick(int frameIndex,int startFrame,int endFrame);
        
#if UNITY_EDITOR
        public virtual void OnEditorPreviewTick(int frameIndex,int startFrame,int endFrame)
        {
        }
#endif
    }
}