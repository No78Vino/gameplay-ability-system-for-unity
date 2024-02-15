namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityString
{
    [TaskCategory("Unity/String")]
    [TaskDescription("Returns success if the string is null or empty")]
    public class IsNullOrEmpty : Conditional
    {
        [Tooltip("The target string")]
        public SharedString targetString;

        public override TaskStatus OnUpdate()
        {
            return string.IsNullOrEmpty(targetString.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetString = "";
        }
    }
}