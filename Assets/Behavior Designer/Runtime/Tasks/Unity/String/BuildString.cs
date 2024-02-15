namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityString
{
    [TaskCategory("Unity/String")]
    [TaskDescription("Creates a string from multiple other strings.")]
    public class BuildString : Action
    {
        [Tooltip("The array of strings")]
        public SharedString[] source;
        [Tooltip("The stored result")]
        [RequiredField]
        public SharedString storeResult;

        public override TaskStatus OnUpdate()
        {
            for (int i = 0; i < source.Length; ++i) {
                storeResult.Value += source[i];
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            source = null;
            storeResult = null;
        }
    }
}