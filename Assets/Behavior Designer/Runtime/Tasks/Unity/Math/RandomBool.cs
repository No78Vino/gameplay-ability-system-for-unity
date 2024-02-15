using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Sets a random bool value")]
    public class RandomBool : Action
    {
        [Tooltip("The variable to store the result")]
        public SharedBool storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Random.value < 0.5f;
            return TaskStatus.Success;
        }
    }
}