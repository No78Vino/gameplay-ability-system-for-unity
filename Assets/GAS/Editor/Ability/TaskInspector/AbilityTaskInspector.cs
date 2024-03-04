using GAS.Runtime.Ability;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability
{
    public abstract class AbilityTaskInspector
    {
        protected AbilityTaskBase _taskBase;

        public AbilityTaskInspector(AbilityTaskBase taskBase)
        {
            _taskBase = taskBase;
        }

        public abstract VisualElement Inspector();

        protected abstract void Save();

        public abstract void OnTargetCatcherPreview(GameObject obj);
        
        
        
        // {
        //     var currentInspectorObject = AbilityTimelineEditorWindow.Instance.CurrentInspectorObject;
        //     (currentInspectorObject as ReleaseGameplayEffectMark)?.MarkDataForSave
        //         .SaveTargetCatcher(_taskBase);
        //     
        //     AbilityTimelineEditorWindow.Instance.Save();
        // }
    }
}