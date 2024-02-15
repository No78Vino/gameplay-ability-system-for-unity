namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedFloat variable to the specified object. Returns Success.")]
    public class SetSharedFloat : Action
    {
        [Tooltip("The value to set the SharedFloat to")]
        public SharedFloat targetValue;
        [RequiredField]
        [Tooltip("The SharedFloat to set")]
        public SharedFloat targetVariable;

        public override TaskStatus OnUpdate()
        {
            targetVariable.Value = targetValue.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = 0;
            targetVariable = 0;
        }
    }
}