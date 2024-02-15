using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Returns success when an object exits the 2D trigger. This task will only receive the physics callback if it is being reevaluated (with a conditional abort or under a parallel task).")]
    [TaskCategory("Physics")]
    public class HasExitedTrigger2D : Conditional
    {
        [Tooltip("The tag of the GameObject to check for a trigger against")]
        public SharedString tag = "";
        [Tooltip("The object that exited the trigger")]
        public SharedGameObject otherGameObject;

        private bool exitedTrigger = false;

        public override TaskStatus OnUpdate()
        {
            return exitedTrigger ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            exitedTrigger = false;
        }

        public override void OnTriggerExit2D(Collider2D other)
        {
            if (string.IsNullOrEmpty(tag.Value) || other.gameObject.CompareTag(tag.Value)) {
                otherGameObject.Value = other.gameObject;
                exitedTrigger = true;
            }
        }

        public override void OnReset()
        {
            tag = "";
            otherGameObject = null;
        }
    }
}
