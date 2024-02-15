using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedVector4 variable to the specified object. Returns Success.")]
    public class SetSharedVector4 : Action
    {
        [Tooltip("The value to set the SharedVector4 to")]
        public SharedVector4 targetValue;
        [RequiredField]
        [Tooltip("The SharedVector4 to set")]
        public SharedVector4 targetVariable;

        public override TaskStatus OnUpdate()
        {
            targetVariable.Value = targetValue.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = Vector4.zero;
            targetVariable = Vector4.zero;
        }
    }
}