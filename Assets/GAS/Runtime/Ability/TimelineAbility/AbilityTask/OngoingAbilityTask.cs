namespace GAS.Runtime
{
    public abstract class OngoingAbilityTask : AbilityTaskBase
    {
#if UNITY_EDITOR
        /// <summary>
        /// 编辑器预览用
        /// 【注意】 覆写时，记得用UNITY_EDITOR宏包裹，这是预览表现用的函数，不该被编译。
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="startFrame"></param>
        /// <param name="endFrame"></param>
        public virtual void OnEditorPreview(int frame, int startFrame, int endFrame)
        {
        }
#endif
        public abstract void OnStart(int startFrame);

        public abstract void OnEnd(int endFrame);

        public abstract void OnTick(int frameIndex, int startFrame, int endFrame);
    }

    public abstract class OngoingAbilityTaskT<T> : OngoingAbilityTask where T : AbilitySpec
    {
        public new T Spec => (T)_spec;
    }
}