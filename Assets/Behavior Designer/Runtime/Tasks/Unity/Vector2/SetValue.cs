using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2
{
    [TaskCategory("Unity/Vector2")]
    [TaskDescription("Sets the value of the Vector2.")]
    public class SetValue : Action
    {
        [Tooltip("The Vector2 to get the values of")]
        public SharedVector2 vector2Value;
        [Tooltip("The Vector2 to set the values of")]
        public SharedVector2 vector2Variable;

        public override TaskStatus OnUpdate()
        {
            vector2Variable.Value = vector2Value.Value;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            vector2Value = Vector2.zero; 
            vector2Variable = Vector2.zero;
        }
    }
}