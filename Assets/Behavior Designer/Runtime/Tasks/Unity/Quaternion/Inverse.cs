using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion
{
    [TaskCategory("Unity/Quaternion")]
    [TaskDescription("Stores the inverse of the specified quaternion.")]
    public class Inverse : Action
    {
        [Tooltip("The target quaternion")]
        public SharedQuaternion targetQuaternion;
        [Tooltip("The stored quaternion")]
        [RequiredField]
        public SharedQuaternion storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Quaternion.Inverse(targetQuaternion.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetQuaternion = storeResult = Quaternion.identity;
        }
    }
}