namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedTransform variable to the specified object. Returns Success.")]
    public class SetSharedTransform : Action
    {
        [Tooltip("The value to set the SharedTransform to. If null the variable will be set to the current Transform")]
        public SharedTransform targetValue;
        [RequiredField]
        [Tooltip("The SharedTransform to set")]
        public SharedTransform targetVariable;

        public override TaskStatus OnUpdate()
        {
            targetVariable.Value = (targetValue.Value != null ? targetValue.Value : transform);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = null;
            targetVariable = null;
        }
    }
}