namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Performs comparison between two floats: less than, less than or equal to, equal to, not equal to, greater than or equal to, or greater than.")]
    public class FloatComparison : Conditional
    {
        public enum Operation
        {
            LessThan,
            LessThanOrEqualTo,
            EqualTo,
            NotEqualTo,
            GreaterThanOrEqualTo,
            GreaterThan
        }

        [Tooltip("The operation to perform")]
        public Operation operation;
        [Tooltip("The first float")]
        public SharedFloat float1;
        [Tooltip("The second float")]
        public SharedFloat float2;

        public override TaskStatus OnUpdate()
        {
            switch (operation) {
                case Operation.LessThan:
                    return float1.Value < float2.Value ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.LessThanOrEqualTo:
                    return float1.Value <= float2.Value ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.EqualTo:
                    return UnityEngine.Mathf.Approximately(float1.Value, float2.Value) ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.NotEqualTo:
                    return !UnityEngine.Mathf.Approximately(float1.Value, float2.Value) ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.GreaterThanOrEqualTo:
                    return float1.Value >= float2.Value ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.GreaterThan:
                    return float1.Value > float2.Value ? TaskStatus.Success : TaskStatus.Failure;
            }
            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            operation = Operation.LessThan;
            float1.Value = 0;
            float2.Value = 0;
        }
    }
}