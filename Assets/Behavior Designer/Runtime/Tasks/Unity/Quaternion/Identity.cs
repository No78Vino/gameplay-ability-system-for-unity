using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion
{
    [TaskCategory("Unity/Quaternion")]
    [TaskDescription("Stores the quaternion identity.")]
    public class Identity : Action
    {
        [Tooltip("The identity")]
        [RequiredField]
        public SharedQuaternion storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Quaternion.identity;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            storeResult = Quaternion.identity;
        }
    }
}