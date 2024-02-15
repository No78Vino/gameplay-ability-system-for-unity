using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedColor variable to the specified object. Returns Success.")]
    public class SetSharedColor : Action
    {
        [Tooltip("The value to set the SharedColor to")]
        public SharedColor targetValue;
        [RequiredField]
        [Tooltip("The SharedColor to set")]
        public SharedColor targetVariable;

        public override TaskStatus OnUpdate()
        {
            targetVariable.Value = targetValue.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = Color.black;
            targetVariable = Color.black;
        }
    }
}