using System;
using GAS.Editor.Ability.AbilityTimelineEditor;
using GAS.Runtime.Ability.TargetCatcher;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability
{
    public abstract class TargetCatcherInspector
    {
        protected TargetCatcherBase _targetCatcherBase;

        public TargetCatcherInspector(TargetCatcherBase targetCatcherBase)
        {
            _targetCatcherBase = targetCatcherBase;
        }

        protected void Save()
        {
            var currentInspectorObject = AbilityTimelineEditorWindow.Instance.CurrentInspectorObject;
            (currentInspectorObject as ReleaseGameplayEffectMark)?.MarkDataForSave
                .SaveTargetCatcher(_targetCatcherBase);
            
            AbilityTimelineEditorWindow.Instance.Save();
        }
        
        public abstract VisualElement Inspector();
    }
    
    public abstract class TargetCatcherInspector<T>:TargetCatcherInspector where T : TargetCatcherBase
    {
        protected T _targetCatcher;

        protected TargetCatcherInspector(TargetCatcherBase targetCatcherBase) : base(targetCatcherBase)
        {
            _targetCatcher = (T) targetCatcherBase;
        }
    }
}