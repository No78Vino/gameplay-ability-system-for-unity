namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedObject variable to the specified object. Returns Success.")]
    public class SetSharedObject : Action
    {
        [Tooltip("The value to set the SharedObject to")]
        public SharedObject targetValue;
        [RequiredField]
        [Tooltip("The SharedTransform to set")]
        public SharedObject targetVariable;

        public override TaskStatus OnUpdate()
        {
            targetVariable.Value = targetValue.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetValue = null;
            targetVariable = null;
        }
    }
}