namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedGameObjectList variable to the specified object. Returns Success.")]
    public class SetSharedGameObjectList : Action
    {
        [Tooltip("The value to set the SharedGameObjectList to.")]
        public SharedGameObjectList targetValue;
        [RequiredField]
        [Tooltip("The SharedGameObjectList to set")]
        public SharedGameObjectList targetVariable;

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