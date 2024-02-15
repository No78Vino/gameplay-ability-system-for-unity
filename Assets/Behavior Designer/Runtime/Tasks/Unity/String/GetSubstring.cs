namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityString
{
    [TaskCategory("Unity/String")]
    [TaskDescription("Stores a substring of the target string")]
    public class GetSubstring : Action
    {
        [Tooltip("The target string")]
        public SharedString targetString;
        [Tooltip("The start substring index")]
        public SharedInt startIndex = 0;
        [Tooltip("The length of the substring. Don't use if -1")]
        public SharedInt length = -1;
        [Tooltip("The stored result")]
        [RequiredField]
        public SharedString storeResult;

        public override TaskStatus OnUpdate()
        {
            if (length.Value != -1) {
                storeResult.Value = targetString.Value.Substring(startIndex.Value, length.Value);
            } else {
                storeResult.Value = targetString.Value.Substring(startIndex.Value);
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetString = "";
            startIndex = 0;
            length = -1;
            storeResult = "";
        }
    }
}