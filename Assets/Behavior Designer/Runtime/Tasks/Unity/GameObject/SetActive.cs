using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Activates/Deactivates the GameObject. Returns Success.")]
    public class SetActive : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("Active state of the GameObject")]
        public SharedBool active;

        public override TaskStatus OnUpdate()
        {
            GetDefaultGameObject(targetGameObject.Value).SetActive(active.Value);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            active = false;
        }
    }
}