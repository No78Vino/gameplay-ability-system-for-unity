using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Clamps the float between two values.")]
    public class FloatClamp : Action
    {
        [Tooltip("The float to clamp")]
        public SharedFloat floatVariable;
        [Tooltip("The maximum value of the float")]
        public SharedFloat minValue;
        [Tooltip("The maximum value of the float")]
        public SharedFloat maxValue;

        public override TaskStatus OnUpdate()
        {
            floatVariable.Value = Mathf.Clamp(floatVariable.Value, minValue.Value, maxValue.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            floatVariable = 0;
            minValue = 0;
            maxValue = 0;
        }
    }
}