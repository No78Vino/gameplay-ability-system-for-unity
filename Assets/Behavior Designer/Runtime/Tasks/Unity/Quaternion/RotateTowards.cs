using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion
{
    [TaskCategory("Unity/Quaternion")]
    [TaskDescription("Stores the quaternion after a rotation.")]
    public class RotateTowards : Action
    {
        [Tooltip("The from rotation")]
        public SharedQuaternion fromQuaternion;
        [Tooltip("The to rotation")]
        public SharedQuaternion toQuaternion;
        [Tooltip("The maximum degrees delta")]
        public SharedFloat maxDeltaDegrees;
        [Tooltip("The stored result")]
        [RequiredField]
        public SharedQuaternion storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Quaternion.RotateTowards(fromQuaternion.Value, toQuaternion.Value, maxDeltaDegrees.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            fromQuaternion = toQuaternion = storeResult = Quaternion.identity;
            maxDeltaDegrees = 0;
        }
    }
}