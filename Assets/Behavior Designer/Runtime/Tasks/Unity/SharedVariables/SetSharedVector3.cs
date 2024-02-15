using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedVector3 variable to the specified object. Returns Success.")]
    public class SetSharedVector3 : Action
    {
        [Tooltip("The value to set the SharedVector3 to")]
        public SharedVector3 targetValue;
        [RequiredField]
        [Tooltip("The SharedVector3 to set")]
        public SharedVector3 targetVariable;

        public override TaskStatus OnUpdate()
        {
            targetVariable.Value = targetValue.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = Vector3.zero;
            targetVariable = Vector3.zero;
        }
    }
}