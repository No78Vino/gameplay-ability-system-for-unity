using UnityEditor;
using UnityEngine;

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachineSameAsFollowTarget))]
    [CanEditMultipleObjects]
    internal sealed class CinemachineSameAsFollowTargetEditor : BaseEditor<CinemachineSameAsFollowTarget>
    {
        public override void OnInspectorGUI()
        {
            BeginInspector();
            bool needWarning = false;
            for (int i = 0; !needWarning && i < targets.Length; ++i)
                needWarning = (targets[i] as CinemachineSameAsFollowTarget).FollowTarget == null;
            if (needWarning)
                EditorGUILayout.HelpBox(
                    "Same As Follow Target requires a Follow target.  It will set the virtual camera's "
                        + "rotation to be the same as that of the Follow Target.",
                    MessageType.Warning);
            DrawRemainingPropertiesInInspector();
        }
    }
}
