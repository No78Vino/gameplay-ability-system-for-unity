using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityTime
{
    [TaskCategory("Unity/Time")]
    [TaskDescription("Returns the time in seconds it took to complete the last frame.")]
    public class GetDeltaTime : Action
    {
        [Tooltip("The variable to store the result")]
        public SharedFloat storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Time.deltaTime;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            storeResult = 0;
        }
    }
}