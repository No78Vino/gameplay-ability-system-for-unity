using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityPhysics
{
    [TaskCategory("Unity/Physics")]
    [TaskDescription("Casts a sphere against all colliders in the scene. Returns success if a collider was hit.")]
    public class SphereCast : Action
    {
        [Tooltip("Starts the spherecast at the GameObject's position. If null the originPosition will be used")]
        public SharedGameObject originGameObject;
        [Tooltip("Starts the sherecast at the position. Only used if originGameObject is null")]
        public SharedVector3 originPosition;
        [Tooltip("The radius of the spherecast")]
        public SharedFloat radius;
        [Tooltip("The direction of the spherecast")]
        public SharedVector3 direction;
        [Tooltip("The length of the spherecast. Set to -1 for infinity")]
        public SharedFloat distance = -1;
        [Tooltip("Selectively ignore colliders")]
        public LayerMask layerMask = -1;
        [Tooltip("Use world or local space. The direction is in world space if no GameObject is specified")]
        public Space space = Space.Self;

        [SharedRequired]
        [Tooltip("Stores the hit object of the spherecast")]
        public SharedGameObject storeHitObject;
        [SharedRequired]
        [Tooltip("Stores the hit point of the spherecast")]
        public SharedVector3 storeHitPoint;
        [SharedRequired]
        [Tooltip("Stores the hit normal of the spherecast")]
        public SharedVector3 storeHitNormal;
        [SharedRequired]
        [Tooltip("Stores the hit distance of the spherecast")]
        public SharedFloat storeHitDistance;

        public override TaskStatus OnUpdate()
        {
            Vector3 position;
            Vector3 dir = direction.Value;
            if (originGameObject.Value != null) {
                position = originGameObject.Value.transform.position;
                if (space == Space.Self) {
                    dir = originGameObject.Value.transform.TransformDirection(direction.Value);
                }
            } else {
                position = originPosition.Value;
            }

            RaycastHit hit;
            if (Physics.SphereCast(position, radius.Value, dir, out hit, distance.Value == -1 ? Mathf.Infinity : distance.Value, layerMask)) {
                storeHitObject.Value = hit.collider.gameObject;
                storeHitPoint.Value = hit.point;
                storeHitNormal.Value = hit.normal;
                storeHitDistance.Value = hit.distance;
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            originGameObject = null;
            originPosition = Vector3.zero;
            radius = 0;
            direction = Vector3.zero;
            distance = -1;
            layerMask = -1;
            space = Space.Self;
        }
    }
}