using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector3
{
    [TaskCategory("Unity/Vector3")]
    [TaskDescription("Performs a math operation on two Vector3s: Add, Subtract, Multiply, Divide, Min, or Max.")]
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
        [Tooltip("The first Vector3")]
        public SharedVector3 firstVector3;
        [Tooltip("The second Vector3")]
        public SharedVector3 secondVector3;
        [Tooltip("The variable to store the result")]
        public SharedVector3 storeResult;

        public override TaskStatus OnUpdate()
        {
            switch (operation) {
                case Operation.Add:
                    storeResult.Value = firstVector3.Value + secondVector3.Value;
                    break;
                case Operation.Subtract:
                    storeResult.Value = firstVector3.Value - secondVector3.Value;
                    break;
                case Operation.Scale:
                    storeResult.Value = Vector3.Scale(firstVector3.Value, secondVector3.Value);
                    break;
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            operation = Operation.Add;
            firstVector3 = Vector3.zero; 
            secondVector3 = Vector3.zero; 
            storeResult = Vector3.zero;
        }
    }
}