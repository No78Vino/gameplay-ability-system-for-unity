#if UNITY_EDITOR
namespace GAS.Editor
{
    using Editor;
    using Runtime;
    
    public abstract class OngoingTaskInspector
    {
        protected OngoingAbilityTask _taskBase;
        public virtual void Init(OngoingAbilityTask task)
        {
            _taskBase = task;
        }
    }
}

#endif