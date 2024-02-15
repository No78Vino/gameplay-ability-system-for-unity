using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedVector2 variable to the specified object. Returns Success.")]
    public class SetSharedVector2 : Action
    {
        [Tooltip("The value to set the SharedVector2 to")]
        public SharedVector2 targetValue;
        [RequiredField]
        [Tooltip("The SharedVector2 to set")]
        public SharedVector2 targetVariable;

        public override TaskStatus OnUpdate()
        {
            targetVariable.Value = targetValue.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = Vector2.zero;
            targetVariable = Vector2.zero;
        }
    }
}