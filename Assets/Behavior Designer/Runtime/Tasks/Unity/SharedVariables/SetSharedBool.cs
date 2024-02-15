namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedBool variable to the specified object. Returns Success.")]
    public class SetSharedBool : Action
    {
        [Tooltip("The value to set the SharedBool to")]
        public SharedBool targetValue;
        [RequiredField]
        [Tooltip("The SharedBool to set")]
        public SharedBool targetVariable;

        public override TaskStatus OnUpdate()
        {
            targetVariable.Value = targetValue.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = false;
            targetVariable = false;
        }
    }
}