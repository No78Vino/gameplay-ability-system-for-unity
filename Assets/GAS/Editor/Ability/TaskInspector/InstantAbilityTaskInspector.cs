#if UNITY_EDITOR

namespace GAS.Editor.Ability
{
    using GAS.Editor.Ability.AbilityTimelineEditor;
    using GAS.Runtime.Ability;
    // public abstract class InstantAbilityTaskInspector:AbilityTaskInspector 
    // {
    //     protected InstantAbilityTaskInspector(AbilityTaskBase taskBase) : base(taskBase)
    //     {
    //     }
    //     
    //     public abstract void OnEditorPreview();
    // }
    //
    // public abstract class InstantAbilityTaskInspector<T>:InstantAbilityTaskInspector where T:InstantAbilityTask
    // {
    //     protected T _task;
    //     protected InstantAbilityTaskInspector(AbilityTaskBase taskBase) : base(taskBase)
    //     {
    //         _task = _taskBase as T;
    //     }
    //     
    //     protected override void Save()
    //     {
    //         var currentInspectorObject = AbilityTimelineEditorWindow.Instance.CurrentInspectorObject;
    //         (currentInspectorObject as TaskMark)?.SaveCurrentTask(_task);
    //         AbilityTimelineEditorWindow.Instance.Save();
    //     }
    // }
    
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