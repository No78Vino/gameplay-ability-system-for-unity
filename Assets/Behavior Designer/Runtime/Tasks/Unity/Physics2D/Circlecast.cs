using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityPhysics2D
{
    [TaskCategory("Unity/Physics2D")]
    [TaskDescription("Casts a circle against all colliders in the scene. Returns success if a collider was hit.")]
    public class Circlecast : Action
    {
        [Tooltip("Starts the circlecast at the GameObject's position. If null the originPosition will be used.")]
        public SharedGameObject originGameObject;
        [Tooltip("Starts the circlecast at the position. Only used if originGameObject is null.")]
        public SharedVector2 originPosition;
        [Tooltip("The radius of the circlecast")]
        public SharedFloat radius;
        [Tooltip("The direction of the circlecast")]
        public SharedVector2 direction;
        [Tooltip("The length of the ray. Set to -1 for infinity.")]
        public SharedFloat distance = -1;
        [Tooltip("Selectively ignore colliders.")]
        public LayerMask layerMask = -1;
        [Tooltip("Use world or local space. The direction is in world space if no GameObject is specified.")]
        public Space space = Space.Self;

        [SharedRequired]
        [Tooltip("Stores the hit object of the circlecast.")]
        public SharedGameObject storeHitObject;
        [SharedRequired]
        [Tooltip("Stores the hit point of the circlecast.")]
        public SharedVector2 storeHitPoint;
        [SharedRequired]
        [Tooltip("Stores the hit normal of the circlecast.")]
        public SharedVector2 storeHitNormal;
        [SharedRequired]
        [Tooltip("Stores the hit distance of the circlecast.")]
        public SharedFloat storeHitDistance;

        public override TaskStatus OnUpdate()
        {
            Vector2 position;
            Vector2 dir = direction.Value;
            if (originGameObject.Value != null) {
                position = originGameObject.Value.transform.position;
                if (space == Space.Self) {
                    dir = originGameObject.Value.transform.TransformDirection(direction.Value);
                }
            } else {
                position = originPosition.Value;
            }

            var hit = Physics2D.CircleCast(position, radius.Value, dir, distance.Value == -1 ? Mathf.Infinity : distance.Value, layerMask);
            if (hit.collider != null) {
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
            originPosition = Vector2.zero;
            direction = Vector2.zero;
            radius = 0;
            distance = -1;
            layerMask = -1;
            space = Space.Self;
        }
    }
}