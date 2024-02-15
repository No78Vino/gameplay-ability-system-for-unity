using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3
{
    [TaskCategory("Unity/Vector3")]
    [TaskDescription("Multiply the Vector3 by a float.")]
    public class Multiply : Action
    {
        [Tooltip("The Vector3 to multiply of")]
        public SharedVector3 vector3Variable;
        [Tooltip("The value to multiply the Vector3 of")]
        public SharedFloat multiplyBy;
        [Tooltip("The multiplication resut")]
        [RequiredField]
        public SharedVector3 storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = vector3Variable.Value * multiplyBy.Value;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            vector3Variable = Vector3.zero; 
            storeResult = Vector3.zero;
            multiplyBy = 0;
        }
    }
}