using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Returns Success if tags match, otherwise Failure.")]
    public class CompareTag : Conditional
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The tag to compare against")]
        public SharedString tag;

        public override TaskStatus OnUpdate()
        {
            return GetDefaultGameObject(targetGameObject.Value).CompareTag(tag.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            tag = "";
        }
    }
}