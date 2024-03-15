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

    public abstract class OngoingTaskInspector<T>:OngoingTaskInspector where T:OngoingAbilityTask
    {
        protected T _task;

        public override void Init(OngoingAbilityTask task)
        {
            base.Init(task);
            _task = _taskBase as T;
        }
        
        protected void Save()
        {
            var currentInspectorObject = AbilityTimelineEditorWindow.Instance.CurrentInspectorObject;
            (currentInspectorObject as TaskClip)?.ClipDataForSave.ongoingTask.Save(_task);
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
}

#endif