namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityString
{
    [TaskCategory("Unity/String")]
    [TaskDescription("Replaces a string with the new string")]
    public class Replace : Action
    {
        [Tooltip("The target string")]
        public SharedString targetString;
        [Tooltip("The old replace")]
        public SharedString oldString;
        [Tooltip("The new string")]
        public SharedString newString;
        [Tooltip("The stored result")]
        [RequiredField]
        public SharedString storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = targetString.Value.Replace(oldString.Value, newString.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetString = "";
            oldString = "";
            newString = "";
            storeResult = "";
        }
    }
}