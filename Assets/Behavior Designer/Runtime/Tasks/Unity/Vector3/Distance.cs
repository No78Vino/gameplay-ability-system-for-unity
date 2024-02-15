using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3
{
    [TaskCategory("Unity/Vector3")]
    [TaskDescription("Returns the distance between two Vector3s.")]
    public class Distance : Action
    {
        [Tooltip("The first Vector3")]
        public SharedVector3 firstVector3;
        [Tooltip("The second Vector3")]
        public SharedVector3 secondVector3;
        [Tooltip("The distance")]
        [RequiredField]
        public SharedFloat storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Vector3.Distance(firstVector3.Value, secondVector3.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            firstVector3 = Vector3.zero;
            secondVector3 = Vector3.zero;
            storeResult = 0;
        }
    }
}
