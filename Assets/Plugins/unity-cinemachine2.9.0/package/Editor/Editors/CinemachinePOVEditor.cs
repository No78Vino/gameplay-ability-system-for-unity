using UnityEditor;

namespace Cinemachine.Editor
{
    [CustomEditor(typeof(CinemachinePOV))]
    [CanEditMultipleObjects]
    internal sealed class CinemachinePOVEditor : BaseEditor<CinemachinePOV>
    {
        private void OnEnable()
        {
            for (int i = 0; i < targets.Length; ++i)
                (targets[i] as CinemachinePOV).UpdateInputAxisProvider();
        }
    }
}
