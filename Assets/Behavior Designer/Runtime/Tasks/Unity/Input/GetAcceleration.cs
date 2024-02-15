using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityInput
{
    [TaskCategory("Unity/Input")]
    [TaskDescription("Stores the acceleration value.")]
    public class GetAcceleration : Action
    {
        [RequiredField]
        [Tooltip("The stored result")]
        public SharedVector3 storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Input.acceleration;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            storeResult = Vector3.zero;
        }
    }
}