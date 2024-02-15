namespace BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables
{
    [TaskCategory("Unity/SharedVariable")]
    [TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class CompareSharedObjectList : Conditional
    {
        [Tooltip("The first variable to compare")]
        public SharedObjectList variable;
        [Tooltip("The variable to compare to")]
        public SharedObjectList compareTo;

        public override TaskStatus OnUpdate()
        {
            if (variable.Value == null && compareTo.Value != null)
                return TaskStatus.Failure;
            if (variable.Value == null && compareTo.Value == null)
                return TaskStatus.Success;
            if (variable.Value.Count != compareTo.Value.Count)
                return TaskStatus.Failure;

            for (int i = 0; i < variable.Value.Count; ++i) {
                if (variable.Value[i] != compareTo.Value[i]) {
                    return TaskStatus.Failure;
                }
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            variable = null;
            compareTo = null;
        }
    }
}