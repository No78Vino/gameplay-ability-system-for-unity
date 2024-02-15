using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2
{
    [TaskCategory("Unity/Vector2")]
    [TaskDescription("Stores the X and Y values of the Vector2.")]
    public class GetXY : Action
    {
        [Tooltip("The Vector2 to get the values of")]
        public SharedVector2 vector2Variable;
        [Tooltip("The X value")]
        [RequiredField]
        public SharedFloat storeX;
        [Tooltip("The Y value")]
        [RequiredField]
        public SharedFloat storeY;

        public override TaskStatus OnUpdate()
        {
            storeX.Value = vector2Variable.Value.x;
            storeY.Value = vector2Variable.Value.y;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            vector2Variable = Vector2.zero;
            storeX = storeY = 0;
        }
    }
}