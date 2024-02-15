using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2
{
    [TaskCategory("Unity/Vector2")]
    [TaskDescription("Performs a math operation on two Vector2s: Add, Subtract, Multiply, Divide, Min, or Max.")]
    public class Operator : Action
    {
        public enum Operation
        {
            Add,
            Subtract,
            Scale
        }

        [Tooltip("The operation to perform")]
        public Operation operation;
        [Tooltip("The first Vector2")]
        public SharedVector2 firstVector2;
        [Tooltip("The second Vector2")]
        public SharedVector2 secondVector2;
        [Tooltip("The variable to store the result")]
        public SharedVector2 storeResult;

        public override TaskStatus OnUpdate()
        {
            switch (operation) {
                case Operation.Add:
                    storeResult.Value = firstVector2.Value + secondVector2.Value;
                    break;
                case Operation.Subtract:
                    storeResult.Value = firstVector2.Value - secondVector2.Value;
                    break;
                case Operation.Scale:
                    storeResult.Value = Vector2.Scale(firstVector2.Value, secondVector2.Value);
                    break;
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            operation = Operation.Add;
            firstVector2 = Vector2.zero;
            secondVector2 = Vector2.zero; 
            storeResult = Vector2.zero;
        }
    }
}