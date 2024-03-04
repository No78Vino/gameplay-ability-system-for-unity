using GAS.Runtime.Ability.TargetCatcher;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability
{
    public class CatchTargetInspector:TargetCatcherInspector<CatchTarget>
    {
        public CatchTargetInspector(TargetCatcherBase targetCatcherBase) : base(targetCatcherBase)
        {
        }

        public override VisualElement Inspector()
        {
            return new VisualElement();
        }

        public override void OnTargetCatcherPreview(GameObject obj)
        {
            Handles.Label(obj.transform.position, _targetCatcher.GetType().Name);
        }
    }
}