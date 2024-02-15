using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityDebug
{
    [TaskCategory("Unity/Debug")]
    [TaskDescription("Draws a debug line")]
    public class DrawLine : Action
    {
        [Tooltip("The start position")]
        public SharedVector3 start;
        [Tooltip("The end position")]
        public SharedVector3 end;
        [Tooltip("The color")]
        public SharedColor color = Color.white;
        [Tooltip("Duration the line will be visible for in seconds.\nDefault: 0 means 1 frame.")]
        public SharedFloat duration;
        [Tooltip("Whether the line should show through world geometry.")]
        public SharedBool depthTest = true;

        public override TaskStatus OnUpdate()
        {
            Debug.DrawLine(start.Value, end.Value, color.Value, duration.Value, depthTest.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            start = Vector3.zero;
            end = Vector3.zero;
            color = Color.white;
            duration = 0f;
            depthTest = true;
        }
    }
}