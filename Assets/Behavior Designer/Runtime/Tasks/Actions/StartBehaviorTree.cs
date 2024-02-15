using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Start a new behavior tree and return success after it has been started.")]
    [TaskIcon("{SkinColor}StartBehaviorTreeIcon.png")]
    public class StartBehaviorTree : Action
    {
        [Tooltip("The GameObject of the behavior tree that should be started. If null use the current behavior")]
        public SharedGameObject behaviorGameObject;
        [Tooltip("The group of the behavior tree that should be started")]
        public SharedInt group;
        [Tooltip("Should this task wait for the behavior tree to complete?")]
        public SharedBool waitForCompletion = false;
        [Tooltip("Should the variables be synchronized?")]
        public SharedBool synchronizeVariables;

        private bool behaviorComplete;
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

            if (behavior != null) {
                var variables = Owner.GetAllVariables();
                if (variables != null && synchronizeVariables.Value) {
                    for (int i = 0; i < variables.Count; ++i) {
                        behavior.SetVariableValue(variables[i].Name, variables[i]);
                    }
                }

                behavior.EnableBehavior();

                if (waitForCompletion.Value) {
                    behaviorComplete = false;
                    behavior.OnBehaviorEnd += BehaviorEnded;
                }
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (behavior == null) {
                return TaskStatus.Failure;
            }

            // Return a status of running if we are waiting for the behavior tree to end and it hasn't ended yet
            if (waitForCompletion.Value && !behaviorComplete) {
                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }

        private void BehaviorEnded(Behavior behavior)
        {
            behaviorComplete = true;
        }

        public override void OnEnd()
        {
            if (behavior != null && waitForCompletion.Value) {
                behavior.OnBehaviorEnd -= BehaviorEnded;
            }
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values.
            behaviorGameObject = null;
            group = 0;
            waitForCompletion = false;
            synchronizeVariables = false;
        }
    }
}