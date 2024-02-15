using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityDebug
{
    [TaskCategory("Unity/Debug")]
    [TaskDescription("Draws a debug ray")]
    public class DrawRay : Action
    {
        [Tooltip("The position")]
        public SharedVector3 start;
        [Tooltip("The direction")]
        public SharedVector3 direction;
        [Tooltip("The color")]
        public SharedColor color = Color.white;

        public override TaskStatus OnUpdate()
        {
            Debug.DrawRay(start.Value, direction.Value, color.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            start = Vector3.zero;
            direction = Vector3.zero;
            color = Color.white;
        }
    }
}