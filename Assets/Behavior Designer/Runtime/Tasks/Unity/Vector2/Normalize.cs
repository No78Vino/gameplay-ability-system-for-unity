using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2
{
    [TaskCategory("Unity/Vector2")]
    [TaskDescription("Normalize the Vector2.")]
    public class Normalize : Action
    {
        [Tooltip("The Vector2 to normalize")]
        public SharedVector2 vector2Variable;
        [Tooltip("The normalized resut")]
        [RequiredField]
        public SharedVector2 storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = vector2Variable.Value.normalized;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            vector2Variable = Vector2.zero; 
            storeResult = Vector2.zero;
        }
    }
}