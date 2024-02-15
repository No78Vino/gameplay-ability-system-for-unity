using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityQuaternion
{
    [TaskCategory("Unity/Quaternion")]
    [TaskDescription("Stores the rotation which rotates the specified degrees around the specified axis.")]
    public class AngleAxis : Action
    {
        [Tooltip("The number of degrees")]
        public SharedFloat degrees;
        [Tooltip("The axis direction")]
        public SharedVector3 axis;
        [Tooltip("The stored result")]
        [RequiredField]
        public SharedQuaternion storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Quaternion.AngleAxis(degrees.Value, axis.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            degrees = 0;
            axis = Vector3.zero;
            storeResult = Quaternion.identity;
        }
    }
}