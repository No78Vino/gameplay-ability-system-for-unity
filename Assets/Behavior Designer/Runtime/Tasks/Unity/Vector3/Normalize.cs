using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3
{
    [TaskCategory("Unity/Vector3")]
    [TaskDescription("Normalize the Vector3.")]
    public class Normalize : Action
    {
        [Tooltip("The Vector3 to normalize")]
        public SharedVector3 vector3Variable;
        [Tooltip("The normalized resut")]
        [RequiredField]
        public SharedVector3 storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Vector3.Normalize(vector3Variable.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            vector3Variable = Vector3.zero; 
            storeResult = Vector3.zero;
        }
    }
}