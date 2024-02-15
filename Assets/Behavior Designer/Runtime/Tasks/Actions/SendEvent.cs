using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Sends an event to the behavior tree, returns success after sending the event.")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer/events/")]
    [TaskIcon("{SkinColor}SendEventIcon.png")]
    public class SendEvent : Action
    {
        [Tooltip("The GameObject of the behavior tree that should have the event sent to it. If null use the current behavior")]
        public SharedGameObject targetGameObject;
        [Tooltip("The event to send")]
        public SharedString eventName;
        [Tooltip("The group of the behavior tree that the event should be sent to")]
        public SharedInt group;
        [Tooltip("Optionally specify a first argument to send")]
        [SharedRequired]
        public SharedVariable argument1;
        [Tooltip("Optionally specify a second argument to send")]
        [SharedRequired]
        public SharedVariable argument2;
        [Tooltip("Optionally specify a third argument to send")]
        [SharedRequired]
        public SharedVariable argument3;

        private BehaviorTree behaviorTree;

        public override void OnStart()
        {
            var behaviorTrees = GetDefaultGameObject(targetGameObject.Value).GetComponents<BehaviorTree>();
            if (behaviorTrees.Length == 1) {
                behaviorTree = behaviorTrees[0];
            } else if (behaviorTrees.Length > 1) {
                for (int i = 0; i < behaviorTrees.Length; ++i) {
                    if (behaviorTrees[i].Group == group.Value) {
                        behaviorTree = behaviorTrees[i];
                        break;
                    }
                }
                // If the group can't be found then use the first behavior tree
                if (behaviorTree == null) {
                    behaviorTree = behaviorTrees[0];
                }
            }
        }

        public override TaskStatus OnUpdate()
        {
            // Send the event and return success
            if (argument1 == null || argument1.IsNone) {
                behaviorTree.SendEvent(eventName.Value);
            } else {
                if (argument2 == null || argument2.IsNone) {
                    behaviorTree.SendEvent<object>(eventName.Value, argument1.GetValue());
                } else {
                    if (argument3 == null || argument3.IsNone) {
                        behaviorTree.SendEvent<object, object>(eventName.Value, argument1.GetValue(), argument2.GetValue());
                    } else {
                        behaviorTree.SendEvent<object, object, object>(eventName.Value, argument1.GetValue(), argument2.GetValue(), argument3.GetValue());
                    }
                }
            }
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            // Reset the properties back to their original values
            targetGameObject = null;
            eventName = "";
        }
    }
}