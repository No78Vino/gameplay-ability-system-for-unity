#if UNITY_EDITOR
namespace GAS.Editor
{
    using Editor;
    using Runtime;
    using UnityEngine;
    using UnityEngine.UIElements;
    
    public abstract class TargetCatcherInspector
    {
        protected TargetCatcherBase _targetCatcherBase;

        public TargetCatcherInspector(TargetCatcherBase targetCatcherBase)
        {
            _targetCatcherBase = targetCatcherBase;
        }

        // protected void Save()
        // {
        //     var currentInspectorObject = AbilityTimelineEditorWindow.Instance.CurrentInspectorObject;
        //     (currentInspectorObject as ReleaseGameplayEffectMark)?.MarkDataForSave
        //         .SaveTargetCatcher(_targetCatcherBase);
        //     
        //     AbilityTimelineEditorWindow.Instance.Save();
        // }
        
        //public abstract VisualElement Inspector();
        //public abstract void OnTargetCatcherPreview(GameObject obj);
    }
    
    public abstract class TargetCatcherInspector<T>:TargetCatcherInspector where T : TargetCatcherBase
    {
        protected T _targetCatcher;

        protected TargetCatcherInspector(TargetCatcherBase targetCatcherBase) : base(targetCatcherBase)
        {
            _targetCatcher = (T) targetCatcherBase;
        }
        
        protected void Save()
        {
            var currentInspectorObject = AbilityTimelineEditorWindow.Instance.CurrentInspectorObject;
            (currentInspectorObject as ReleaseGameplayEffectMark)?.MarkDataForSave
                .SaveTargetCatcher(_targetCatcher);
            
            AbilityTimelineEditorWindow.Instance.Save();
        }
    }
}
#endif