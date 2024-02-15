using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class CompareSharedRect : Conditional
    {
        [Tooltip("The first variable to compare")]
        public SharedRect variable;
        [Tooltip("The variable to compare to")]
        public SharedRect compareTo;

        public override TaskStatus OnUpdate()
        {
            return variable.Value.Equals(compareTo.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            variable = new Rect();
            compareTo = new Rect();
        }
    }
}