namespace GAS.Runtime
{
    public abstract class InstantAbilityTask : AbilityTaskBase
    {
#if UNITY_EDITOR
        /// <summary>
        ///  编辑器预览用
        ///     【注意】 覆写时，记得用UNITY_EDITOR宏包裹，这是预览表现用的函数，不该被编译。
        /// </summary>
        public virtual void OnEditorPreview()
        {
        }
#endif
        public abstract void OnExecute();
    }

    public abstract class InstantAbilityTaskT<T> : InstantAbilityTask where T : AbilitySpec
    {
        public new T Spec => (T)_spec;
    }
}