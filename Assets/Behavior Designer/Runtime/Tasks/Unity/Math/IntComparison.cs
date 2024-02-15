namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Performs comparison between two integers: less than, less than or equal to, equal to, not equal to, greater than or equal to, or greater than.")]
    public class IntComparison : Conditional
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
        [Tooltip("The first integer")]
        public SharedInt integer1;
        [Tooltip("The second integer")]
        public SharedInt integer2;

        public override TaskStatus OnUpdate()
        {
            switch (operation) {
                case Operation.LessThan:
                    return integer1.Value < integer2.Value ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.LessThanOrEqualTo:
                    return integer1.Value <= integer2.Value ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.EqualTo:
                    return integer1.Value == integer2.Value ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.NotEqualTo:
                    return integer1.Value != integer2.Value ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.GreaterThanOrEqualTo:
                    return integer1.Value >= integer2.Value ? TaskStatus.Success : TaskStatus.Failure;
                case Operation.GreaterThan:
                    return integer1.Value > integer2.Value ? TaskStatus.Success : TaskStatus.Failure;
            }
            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            operation = Operation.LessThan;
            integer1.Value = 0;
            integer2.Value = 0;
        }
    }
}