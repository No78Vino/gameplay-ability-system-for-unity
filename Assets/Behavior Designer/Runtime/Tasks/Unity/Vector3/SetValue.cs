using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3
{
    [TaskCategory("Unity/Vector3")]
    [TaskDescription("Sets the value of the Vector3.")]
    public class SetValue : Action
    {
        [Tooltip("The Vector3 to get the values of")]
        public SharedVector3 vector3Value;
        [Tooltip("The Vector3 to set the values of")]
        public SharedVector3 vector3Variable;

        public override TaskStatus OnUpdate()
        {
            vector3Variable.Value = vector3Value.Value;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            vector3Value = Vector3.zero; 
            vector3Variable = Vector3.zero;
        }
    }
}