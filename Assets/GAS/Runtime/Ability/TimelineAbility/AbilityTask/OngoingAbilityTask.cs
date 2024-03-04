namespace GAS.Runtime.Ability
{
    public abstract class OngoingAbilityTask:AbilityTaskBase
    {
#if UNITY_EDITOR
        public virtual void OnEditorPreviewTick(int frameIndex,int startFrame,int endFrame)
        {
        }
#endif
        public abstract void OnStart(int startFrame);

        public abstract void OnEnd(int endFrame);

        public abstract void OnTick(int frameIndex,int startFrame,int endFrame);
    }
}