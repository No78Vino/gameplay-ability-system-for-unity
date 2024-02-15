using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class CompareSharedVector3 : Conditional
    {
        [Tooltip("The first variable to compare")]
        public SharedVector3 variable;
        [Tooltip("The variable to compare to")]
        public SharedVector3 compareTo;

        public override TaskStatus OnUpdate()
        {
            return variable.Value.Equals(compareTo.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            variable = Vector3.zero;
            compareTo = Vector3.zero;
        }
    }
}