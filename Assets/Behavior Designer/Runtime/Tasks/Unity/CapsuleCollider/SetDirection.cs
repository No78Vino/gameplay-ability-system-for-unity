using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCapsuleCollider
{
    [TaskCategory("Unity/CapsuleCollider")]
    [TaskDescription("Sets the direction of the CapsuleCollider. Returns Success.")]
    public class SetDirection : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The direction of the CapsuleCollider")]
        public SharedInt direction;

        private CapsuleCollider capsuleCollider;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                capsuleCollider = currentGameObject.GetComponent<CapsuleCollider>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (capsuleCollider == null) {
                Debug.LogWarning("CapsuleCollider is null");
                return TaskStatus.Failure;
            }

            capsuleCollider.direction = direction.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            direction = 0;
        }
    }
}