using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Sends a message to the target GameObject. Returns Success.")]
    public class SendMessage : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The message to send")]
        public SharedString message;
        [Tooltip("The value to send")]
        public SharedGenericVariable value;

        public override TaskStatus OnUpdate()
        {
            if (value.Value != null) {
                GetDefaultGameObject(targetGameObject.Value).SendMessage(message.Value, value.Value.value.GetValue());
            } else {
                GetDefaultGameObject(targetGameObject.Value).SendMessage(message.Value);
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            message = "";
        }
    }
}