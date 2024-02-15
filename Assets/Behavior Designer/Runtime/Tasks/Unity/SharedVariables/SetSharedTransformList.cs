namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Sets the SharedTransformList variable to the specified object. Returns Success.")]
    public class SetSharedTransformList : Action
    {
        [Tooltip("The value to set the SharedTransformList to.")]
        public SharedTransformList targetValue;
        [RequiredField]
        [Tooltip("The SharedTransformList to set")]
        public SharedTransformList targetVariable;

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