using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Sets a random int value")]
    public class RandomInt : Action
    {
        [Tooltip("The minimum amount")]
        public SharedInt min;
        [Tooltip("The maximum amount")]
        public SharedInt max;
        [Tooltip("Is the maximum value inclusive?")]
        public bool inclusive;
        [Tooltip("The variable to store the result")]
        public SharedInt storeResult;

        public override TaskStatus OnUpdate()
        {
            if (inclusive) {
                storeResult.Value = Random.Range(min.Value, max.Value + 1);
            } else {
                storeResult.Value = Random.Range(min.Value, max.Value);
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            min.Value = 0;
            max.Value = 0;
            inclusive = false;
            storeResult = 0;
        }
    }
}