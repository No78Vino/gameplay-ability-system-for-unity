using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedRect variable to the specified object. Returns Success.")]
    public class SetSharedRect : Action
    {
        [Tooltip("The value to set the SharedRect to")]
        public SharedRect targetValue;
        [RequiredField]
        [Tooltip("The SharedRect to set")]
        public SharedRect targetVariable;

        public override TaskStatus OnUpdate()
        {
            targetVariable.Value = targetValue.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = new Rect();
            targetVariable = new Rect();
        }
    }
}