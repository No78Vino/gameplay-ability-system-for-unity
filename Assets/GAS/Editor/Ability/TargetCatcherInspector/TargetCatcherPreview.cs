#if UNITY_EDITOR
using GAS.Editor.Ability.AbilityTimelineEditor;
using UnityEditor;
using UnityEngine;
namespace GAS.Editor.Ability
{
    public class TargetCatcherPreview : MonoBehaviour
    {
        private TargetCatcherInspector _targetCatcherInspector;

        public void Init(TargetCatcherInspector targetCatcherInspector)
        {
            _targetCatcherInspector = targetCatcherInspector;
        }

        private void OnDrawGizmos()
        {
            if (AbilityTimelineEditorWindow.Instance.AbilityAsset == null ||
                AbilityTimelineEditorWindow.Instance.PreviewObject == null) return;

            _targetCatcherInspector?.OnTargetCatcherPreview(gameObject);
            SceneView.RepaintAll();
        }
    }
}
#endif