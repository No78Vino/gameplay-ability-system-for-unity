using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion
{
    [TaskCategory("Unity/Quaternion")]
    [TaskDescription("Stores the quaternion of a euler vector.")]
    public class Euler : Action
    {
        [Tooltip("The euler vector")]
        public SharedVector3 eulerVector;
        [Tooltip("The stored quaternion")]
        [RequiredField]
        public SharedQuaternion storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Quaternion.Euler(eulerVector.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            eulerVector = Vector3.zero;
            storeResult = Quaternion.identity;
        }
    }
}