#if UNITY_EDITOR
namespace GAS.Editor.Ability
{
    using GAS.Runtime.Ability.TargetCatcher;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    
    public class CatchSelfInspector:TargetCatcherInspector<CatchSelf>
    {
        public CatchSelfInspector(CatchSelf targetCatcherBase) : base(targetCatcherBase)
        {
        }

        public override VisualElement Inspector()
        {
            return new VisualElement();
        }

        public override void OnTargetCatcherPreview(GameObject obj)
        {
            // Gizmos.color = new Color(0, 1, 1, 0.5f);
            // var position = previewObject.transform.position + (Vector3)_targetCatcher.offset;
            // Gizmos.DrawCube(position, (Vector3)_targetCatcher.size, (Vector3)_targetCatcher.rotation);
            Handles.Label(obj.transform.position, _targetCatcher.GetType().Name);
        }
    }
}
#endif