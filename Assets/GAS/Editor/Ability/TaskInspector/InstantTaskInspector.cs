#if UNITY_EDITOR

namespace GAS.Editor
{
    using Editor;
    using Runtime;
    public abstract class InstantTaskInspector
    {
        protected InstantAbilityTask _taskBase;
        public virtual void Init(InstantAbilityTask task)
        {
            _taskBase = task;
        }
    }
}

#endif