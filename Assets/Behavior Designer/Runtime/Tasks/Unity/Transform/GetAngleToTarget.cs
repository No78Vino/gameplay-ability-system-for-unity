using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform
{
    [TaskCategory("Unity/Transform")]
    [TaskDescription("Gets the Angle between a GameObject's forward direction and a target. Returns Success.")]
    public class GetAngleToTarget : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The target object to measure the angle to. If null the targetPosition will be used.")]
        public SharedGameObject targetObject;
        [Tooltip("The world position to measure an angle to. If the targetObject is also not null, this value is used as an offset from that object's position.")]
        public SharedVector3 targetPosition;
        [Tooltip("Ignore height differences when calculating the angle?")]
        public SharedBool ignoreHeight = true;
        [Tooltip("The angle to the target")]
        [RequiredField]
        public SharedFloat storeValue;

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

            Vector3 targetPos;
            if (targetObject.Value != null) {
                targetPos = targetObject.Value.transform.InverseTransformPoint(targetPosition.Value);
            } else {
                targetPos = targetPosition.Value;
            }

            if (ignoreHeight.Value) {
                targetPos.y = targetTransform.position.y;
            }

            var targetDir = targetPos - targetTransform.position;
            storeValue.Value = Vector3.Angle(targetDir, targetTransform.forward);

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            targetObject = null;
            targetPosition = Vector3.zero;
            ignoreHeight = true;
            storeValue = 0;
        }
    }
}