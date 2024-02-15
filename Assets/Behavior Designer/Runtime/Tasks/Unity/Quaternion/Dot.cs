using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion
{
    [TaskCategory("Unity/Quaternion")]
    [TaskDescription("Stores the dot product between two rotations.")]
    public class Dot : Action
    {
        [Tooltip("The first rotation")]
        public SharedQuaternion leftRotation;
        [Tooltip("The second rotation")]
        public SharedQuaternion rightRotation;
        [Tooltip("The stored result")]
        [RequiredField]
        public SharedFloat storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Quaternion.Dot(leftRotation.Value, rightRotation.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            leftRotation = rightRotation = Quaternion.identity;
            storeResult = 0;
        }
    }
}