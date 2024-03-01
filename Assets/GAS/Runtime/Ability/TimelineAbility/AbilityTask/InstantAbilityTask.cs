namespace GAS.Runtime.Ability.TimelineAbility.AbilityTask
{
    public abstract class InstantAbilityTask:AbilityTaskBase
    {
        public abstract void OnExecute();

#if UNITY_EDITOR
        public virtual void OnEditorPreview()
        {
        }
#endif
    }
}