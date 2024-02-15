using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Returns Success if the GameObject is active in the hierarchy, otherwise Failure.")]
    public class ActiveInHierarchy : Conditional
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;

        public override TaskStatus OnUpdate()
        {
            return GetDefaultGameObject(targetGameObject.Value).activeInHierarchy ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetGameObject = null;
        }
    }
}