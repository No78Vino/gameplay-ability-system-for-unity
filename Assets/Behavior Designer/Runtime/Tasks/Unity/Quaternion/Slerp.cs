using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion
{
    [TaskCategory("Unity/Quaternion")]
    [TaskDescription("Spherically lerp between two quaternions.")]
    public class Slerp : Action
    {
        [Tooltip("The from rotation")]
        public SharedQuaternion fromQuaternion;
        [Tooltip("The to rotation")]
        public SharedQuaternion toQuaternion;
        [Tooltip("The amount to lerp")]
        public SharedFloat amount;
        [Tooltip("The stored result")]
        [RequiredField]
        public SharedQuaternion storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Quaternion.Slerp(fromQuaternion.Value, toQuaternion.Value, amount.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            fromQuaternion = toQuaternion = storeResult = Quaternion.identity;
            amount = 0;
        }
    }
}