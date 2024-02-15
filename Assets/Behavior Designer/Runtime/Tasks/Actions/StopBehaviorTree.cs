using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Pause or disable a behavior tree and return success after it has been stopped.")]
    [TaskIcon("{SkinColor}StopBehaviorTreeIcon.png")]
    public class StopBehaviorTree : Action
    {
        [Tooltip("The GameObject of the behavior tree that should be stopped. If null use the current behavior")]
        public SharedGameObject behaviorGameObject;
        [Tooltip("The group of the behavior tree that should be stopped")]
        public SharedInt group;
        [Tooltip("Should the behavior be paused or completely disabled")]
        public SharedBool pauseBehavior = false;

        private Behavior behavior;

        public override void OnStart()
        {
            var behaviorTrees = GetDefaultGameObject(behaviorGameObject.Value).GetComponents<Behavior>();
            if (behaviorTrees.Length == 1) {
                behavior = behaviorTrees[0];
            } else if (behaviorTrees.Length > 1) {
                for (int i = 0; i < behaviorTrees.Length; ++i) {
                    if (behaviorTrees[i].Group == group.Value) {
                        behavior = behaviorTrees[i];
                        break;
                    }
                }
                // If the group can't be found then use the first behavior tree
                if (behavior == null) {
                    behavior = behaviorTrees[0];
                }
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (behavior == null) {
                return TaskStatus.Failure;
            }

            // Start the behavior and return success.
            behavior.DisableBehavior(pauseBehavior.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values
            behaviorGameObject = null;
            group = 0;
            pauseBehavior = false;
        }
    }
}