using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Restarts a behavior tree, returns success after it has been restarted.")]
    [TaskIcon("{SkinColor}RestartBehaviorTreeIcon.png")]
    public class RestartBehaviorTree : Action
    {
        [Tooltip("The GameObject of the behavior tree that should be restarted. If null use the current behavior")]
        public SharedGameObject behaviorGameObject;
        [Tooltip("The group of the behavior tree that should be restarted")]
        public SharedInt group;

        private Behavior behavior;

        public override void OnAwake()
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

            // Stop the behavior tree
            behavior.DisableBehavior();
            // Start the behavior tree back up
            behavior.EnableBehavior();
            // Return success
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values.
            behavior = null;
        }
    }
}