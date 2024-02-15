namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Flips the value of the bool.")]
    public class BoolFlip : Action
    {
        [Tooltip("The bool to flip the value of")]
        public SharedBool boolVariable;

        public override TaskStatus OnUpdate()
        {
            boolVariable.Value = !boolVariable.Value;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            boolVariable.Value = false;
        }
    }
}