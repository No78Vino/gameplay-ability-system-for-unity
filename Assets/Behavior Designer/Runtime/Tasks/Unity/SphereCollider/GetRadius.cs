using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnitySphereCollider
{
    [TaskCategory("Unity/SphereCollider")]
    [TaskDescription("Stores the radius of the SphereCollider. Returns Success.")]
    public class GetRadius : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The radius of the SphereCollider")]
        [RequiredField]
        public SharedFloat storeValue;

        private SphereCollider sphereCollider;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                sphereCollider = currentGameObject.GetComponent<SphereCollider>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (sphereCollider == null) {
                Debug.LogWarning("SphereCollider is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = sphereCollider.radius;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            storeValue = 0;
        }
    }
}