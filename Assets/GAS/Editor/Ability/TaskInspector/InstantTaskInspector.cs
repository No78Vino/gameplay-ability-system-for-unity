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

    public abstract class InstantTaskInspector<T>:InstantTaskInspector where T:InstantAbilityTask
    {
        protected T _task;

        public override void Init(InstantAbilityTask task)
        {
            base.Init(task);
            _task = _taskBase as T;
        }
        
        protected void Save()
        {
            var currentInspectorObject = AbilityTimelineEditorWindow.Instance.CurrentInspectorObject;
            (currentInspectorObject as TaskMark)?.SaveCurrentTask(_task);
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
}

#endif