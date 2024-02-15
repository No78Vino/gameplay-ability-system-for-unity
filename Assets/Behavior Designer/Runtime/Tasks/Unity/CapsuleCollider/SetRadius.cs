using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityCapsuleCollider
{
    [TaskCategory("Unity/CapsuleCollider")]
    [TaskDescription("Sets the radius of the CapsuleCollider. Returns Success.")]
    public class SetRadius : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The radius of the CapsuleCollider")]
        public SharedFloat radius;

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

            capsuleCollider.radius = radius.Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            radius = 0;
        }
    }
}