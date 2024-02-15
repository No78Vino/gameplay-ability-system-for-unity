using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class CompareSharedTransform : Conditional
    {
        [Tooltip("The first variable to compare")]
        public SharedTransform variable;
        [Tooltip("The variable to compare to")]
        public SharedTransform compareTo;

        public override TaskStatus OnUpdate()
        {
            if (variable.Value == null && compareTo.Value != null)
                return TaskStatus.Failure;
            if (variable.Value == null && compareTo.Value == null)
                return TaskStatus.Success;

            return variable.Value.Equals(compareTo.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            variable = null;
            compareTo = null;
        }
    }
}