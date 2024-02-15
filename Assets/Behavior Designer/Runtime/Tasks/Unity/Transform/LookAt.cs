using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform
{
    [TaskCategory("Unity/Transform")]
    [TaskDescription("Rotates the transform so the forward vector points at worldPosition. Returns Success.")]
    public class LookAt : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The GameObject to look at. If null the world position will be used.")]
        public SharedGameObject targetLookAt;
        [Tooltip("Point to look at")]
        public SharedVector3 worldPosition;
        [Tooltip("Vector specifying the upward direction")]
        public Vector3 worldUp;

        private Transform targetTransform;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                targetTransform = currentGameObject.GetComponent<Transform>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (targetTransform == null) {
                Debug.LogWarning("Transform is null");
                return TaskStatus.Failure;
            }

            if (targetLookAt.Value != null) {
                targetTransform.LookAt(targetLookAt.Value.transform);
            } else {
                targetTransform.LookAt(worldPosition.Value, worldUp);
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            targetLookAt = null;
            worldPosition = Vector3.up;
            worldUp = Vector3.up;
        }
    }
}