namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedString variable to the specified object. Returns Success.")]
    public class SetSharedString : Action
    {
        [Tooltip("The value to set the SharedString to")]
        public SharedString targetValue;
        [RequiredField]
        [Tooltip("The SharedString to set")]
        public SharedString targetVariable;

        public override TaskStatus OnUpdate()
        {
            targetVariable.Value = targetValue.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = "";
            targetVariable = "";
        }
    }
}