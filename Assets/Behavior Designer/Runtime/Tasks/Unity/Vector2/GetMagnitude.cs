using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2
{
    [TaskCategory("Unity/Vector2")]
    [TaskDescription("Stores the magnitude of the Vector2.")]
    public class GetMagnitude : Action
    {
        [Tooltip("The Vector2 to get the magnitude of")]
        public SharedVector2 vector2Variable;
        [Tooltip("The magnitude of the vector")]
        [RequiredField]
        public SharedFloat storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = vector2Variable.Value.magnitude;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            vector2Variable = Vector2.zero;
            storeResult = 0;
        }
    }
}