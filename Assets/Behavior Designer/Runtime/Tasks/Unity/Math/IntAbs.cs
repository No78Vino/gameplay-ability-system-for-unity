using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Stores the absolute value of the int.")]
    public class IntAbs : Action
    {
        [Tooltip("The int to return the absolute value of")]
        public SharedInt intVariable;

        public override TaskStatus OnUpdate()
        {
            intVariable.Value = Mathf.Abs(intVariable.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            intVariable = 0;
        }
    }
}