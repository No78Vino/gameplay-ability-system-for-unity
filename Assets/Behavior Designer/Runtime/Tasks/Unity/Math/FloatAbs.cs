using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Stores the absolute value of the float.")]
    public class FloatAbs : Action
    {
        [Tooltip("The float to return the absolute value of")]
        public SharedFloat floatVariable;

        public override TaskStatus OnUpdate()
        {
            floatVariable.Value = Mathf.Abs(floatVariable.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            floatVariable = 0;
        }
    }
}