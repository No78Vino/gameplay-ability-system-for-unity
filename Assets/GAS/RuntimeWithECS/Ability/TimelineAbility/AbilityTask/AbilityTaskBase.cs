namespace GAS.Runtime
{
    public abstract class AbilityTaskBase
    {
        protected AbilityLogicBase _logic;
        public AbilityLogicBase Logic => _logic;

        public virtual void Init(AbilityLogicBase logic)
        {
            _logic = logic;
        }

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
        public virtual void OnStart(int startFrame)
        {
        }

        public virtual void OnEnd(int endFrame)
        {
        }

        public virtual void OnTick(int frameIndex, int startFrame, int endFrame)
        {
        }
    }

    public abstract class AbilityTaskBase<T> : AbilityTaskBase where T : AbilityLogicBase
    {
        public new T Logic => (T)_logic;
    }
}