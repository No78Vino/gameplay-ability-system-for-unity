using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2
{
    [TaskCategory("Unity/Vector2")]
    [TaskDescription("Stores the Vector3 value of the Vector2.")]
    public class GetVector3 : Action
    {
        [Tooltip("The Vector2 to get the Vector3 value of")]
        public SharedVector2 vector3Variable;
        [Tooltip("The Vector3 value")]
        [RequiredField]
        public SharedVector3 storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = vector3Variable.Value;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            vector3Variable = Vector2.zero;
            storeResult = Vector3.zero;
        }
    }
}