using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Clamps the int between two values.")]
    public class IntClamp : Action
    {
        [Tooltip("The int to clamp")]
        public SharedInt intVariable;
        [Tooltip("The maximum value of the int")]
        public SharedInt minValue;
        [Tooltip("The maximum value of the int")]
        public SharedInt maxValue;

        public override TaskStatus OnUpdate()
        {
            intVariable.Value = Mathf.Clamp(intVariable.Value, minValue.Value, maxValue.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            intVariable = 0;
            minValue = 0;
            maxValue = 0;
        }
    }
}