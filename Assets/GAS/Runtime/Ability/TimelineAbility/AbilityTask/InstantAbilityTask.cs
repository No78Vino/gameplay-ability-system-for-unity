namespace GAS.Runtime.Ability
{
    public abstract class InstantAbilityTask:AbilityTaskBase
    {
#if UNITY_EDITOR
        public virtual void OnEditorPreview()
        {
        }
#endif
        public abstract void OnExecute();
    }
}