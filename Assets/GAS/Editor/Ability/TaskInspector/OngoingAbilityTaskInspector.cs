using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.Runtime.Ability;

namespace GAS.Editor.Ability
{
    public abstract class OngoingAbilityTaskInspector : AbilityTaskInspector
    {
        protected OngoingAbilityTaskInspector(AbilityTaskBase taskBase) : base(taskBase)
        {
        }
        
        public abstract void OnEditorPreview(int frame,int startFrame,int endFrame);
    }
    
    public abstract class OngoingAbilityTaskInspector<T>:OngoingAbilityTaskInspector where T:OngoingAbilityTask
    {
        protected T _task;
        protected OngoingAbilityTaskInspector(AbilityTaskBase taskBase) : base(taskBase)
        {
            _task = _taskBase as T;
        }
        
        protected override void Save()
        {
            var currentInspectorObject = AbilityTimelineEditorWindow.Instance.CurrentInspectorObject;
            (currentInspectorObject as TaskClip)?.ClipDataForSave.ongoingTask.Save(_task);
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
}