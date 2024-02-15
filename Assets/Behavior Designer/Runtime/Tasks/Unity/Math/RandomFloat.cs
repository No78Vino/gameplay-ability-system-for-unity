using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Sets a random float value")]
    public class RandomFloat : Action
    {
        [Tooltip("The minimum amount")]
        public SharedFloat min;
        [Tooltip("The maximum amount")]
        public SharedFloat max;
        [Tooltip("Is the maximum value inclusive?")]
        public bool inclusive;
        [Tooltip("The variable to store the result")]
        public SharedFloat storeResult;

        public override TaskStatus OnUpdate()
        {
            if (inclusive) {
                storeResult.Value = Random.Range(min.Value, max.Value);
            } else {
                storeResult.Value = Random.Range(min.Value, max.Value - 0.00001f);
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