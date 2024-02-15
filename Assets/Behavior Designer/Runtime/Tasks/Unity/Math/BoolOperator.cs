namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Performs a math operation on two bools: AND, OR, NAND, or XOR.")]
    public class BoolOperator : Action
    {
        public enum Operation
        {
            AND,
            OR,
            NAND,
            XOR
        }

        [Tooltip("The operation to perform")]
        public Operation operation;
        [Tooltip("The first bool")]
        public SharedBool bool1;
        [Tooltip("The second bool")]
        public SharedBool bool2;
        [Tooltip("The variable to store the result")]
        public SharedBool storeResult;

        public override TaskStatus OnUpdate()
        {
            switch (operation) {
                case Operation.AND:
                    storeResult.Value = bool1.Value && bool2.Value;
                    break;
                case Operation.OR:
                    storeResult.Value = bool1.Value || bool2.Value;
                    break;
                case Operation.NAND:
                    storeResult.Value = !(bool1.Value && bool2.Value);
                    break;
                case Operation.XOR:
                    storeResult.Value = bool1.Value ^ bool2.Value;
                    break;
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            operation = Operation.AND;
            bool1 = false;
            bool2 = false;
        }
    }
}