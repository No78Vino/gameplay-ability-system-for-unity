using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Returns success when a 2D collision ends. This task will only receive the physics callback if it is being reevaluated (with a conditional abort or under a parallel task).")]
    [TaskCategory("Physics")]
    public class HasExitedCollision2D : Conditional
    {
        [Tooltip("The tag of the GameObject to check for a collision against")]
        public SharedString tag = "";
        [Tooltip("The object that exited the collision")]
        public SharedGameObject collidedGameObject;

        private bool exitedCollision = false;

        public override TaskStatus OnUpdate()
        {
            return exitedCollision ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnEnd()
        {
            exitedCollision = false;
        }

        public override void OnCollisionExit2D(Collision2D collision)
        {
            if (string.IsNullOrEmpty(tag.Value) || collision.gameObject.CompareTag(tag.Value)) {
                collidedGameObject.Value = collision.gameObject;
                exitedCollision = true;
            }
        }

        public override void OnReset()
        {
            tag = "";
            collidedGameObject = null;
        }
    }
}
