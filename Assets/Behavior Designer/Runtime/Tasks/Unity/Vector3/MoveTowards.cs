using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3
{
    [TaskCategory("Unity/Vector3")]
    [TaskDescription("Move from the current position to the target position.")]
    public class MoveTowards : Action
    {
        [Tooltip("The current position")]
        public SharedVector3 currentPosition;
        [Tooltip("The target position")]
        public SharedVector3 targetPosition;
        [Tooltip("The movement speed")]
        public SharedFloat speed;
        [Tooltip("The move resut")]
        [RequiredField]
        public SharedVector3 storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Vector3.MoveTowards(currentPosition.Value, targetPosition.Value, speed.Value * Time.deltaTime);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            currentPosition = Vector3.zero; 
            targetPosition = Vector3.zero;
            storeResult = Vector3.zero;
            speed = 0;
        }
    }
}