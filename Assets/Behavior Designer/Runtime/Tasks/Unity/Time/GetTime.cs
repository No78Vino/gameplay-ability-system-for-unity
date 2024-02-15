using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityTime
{
    [TaskCategory("Unity/Time")]
    [TaskDescription("Returns the time in second since the start of the game.")]
    public class GetTime : Action
    {
        [Tooltip("The variable to store the result")]
        public SharedFloat storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Time.time;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            storeResult = 0;
        }
    }
}