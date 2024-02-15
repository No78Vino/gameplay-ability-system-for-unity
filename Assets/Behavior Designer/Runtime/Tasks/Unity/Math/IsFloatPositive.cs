namespace BehaviorDesigner.Runtime.Tasks.Unity.Math
{
    [TaskCategory("Unity/Math")]
    [TaskDescription("Is the float a positive value?")]
    public class IsFloatPositive : Conditional
    {
        [Tooltip("The float to check if positive")]
        public SharedFloat floatVariable;

        public override TaskStatus OnUpdate()
        {
            return floatVariable.Value > 0 ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            floatVariable = 0;
        }
    }
}