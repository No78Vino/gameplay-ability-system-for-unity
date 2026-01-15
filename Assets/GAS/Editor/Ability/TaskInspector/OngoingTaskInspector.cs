#if UNITY_EDITOR
namespace GAS.Editor
{
    using Editor;
    using Runtime;
    
    public abstract class OngoingTaskInspector
    {
        protected AbilityTaskBase _taskBase;
        public virtual void Init(AbilityTaskBase task)
        {
            _taskBase = task;
        }
    }
}

#endif