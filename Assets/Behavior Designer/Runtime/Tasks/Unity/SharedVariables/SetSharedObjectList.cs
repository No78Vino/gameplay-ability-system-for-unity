namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedObjectList variable to the specified object. Returns Success.")]
    public class SetSharedObjectList : Action
    {
        [Tooltip("The value to set the SharedObjectList to.")]
        public SharedObjectList targetValue;
        [RequiredField]
        [Tooltip("The SharedObjectList to set")]
        public SharedObjectList targetVariable;

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