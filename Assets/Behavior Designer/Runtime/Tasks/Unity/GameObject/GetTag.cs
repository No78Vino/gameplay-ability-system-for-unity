using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Stores the GameObject tag. Returns Success.")]
    public class GetTag : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("Active state of the GameObject")]
        [RequiredField]
        public SharedString storeValue;

        public override TaskStatus OnUpdate()
        {
            storeValue.Value = GetDefaultGameObject(targetGameObject.Value).tag;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = "";
        }
    }
}