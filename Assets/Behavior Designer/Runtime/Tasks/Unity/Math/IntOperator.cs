using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Performs a math operation on two integers: Add, Subtract, Multiply, Divide, Min, or Max.")]
    public class IntOperator : Action
    {
        public enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Min,
            Max,
            Modulo
        }

        [Tooltip("The operation to perform")]
        public Operation operation;
        [Tooltip("The first integer")]
        public SharedInt integer1;
        [Tooltip("The second integer")]
        public SharedInt integer2;
        [RequiredField]
        [Tooltip("The variable to store the result")]
        public SharedInt storeResult;

        public override TaskStatus OnUpdate()
        {
            switch (operation) {
                case Operation.Add:
                    storeResult.Value = integer1.Value + integer2.Value;
                    break;
                case Operation.Subtract:
                    storeResult.Value = integer1.Value - integer2.Value;
                    break;
                case Operation.Multiply:
                    storeResult.Value = integer1.Value * integer2.Value;
                    break;
                case Operation.Divide:
                    storeResult.Value = integer1.Value / integer2.Value;
                    break;
                case Operation.Min:
                    storeResult.Value = Mathf.Min(integer1.Value, integer2.Value);
                    break;
                case Operation.Max:
                    storeResult.Value = Mathf.Max(integer1.Value, integer2.Value);
                    break;
                case Operation.Modulo:
                    storeResult.Value = integer1.Value % integer2.Value;
                    break;
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            operation = Operation.Add;
            integer1 = 0;
            integer2 = 0;
            storeResult = 0;
        }
    }
}