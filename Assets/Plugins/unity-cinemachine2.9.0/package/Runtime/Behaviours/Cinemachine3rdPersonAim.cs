#if !UNITY_2019_3_OR_NEWER
#define CINEMACHINE_PHYSICS
#endif

using UnityEngine;

namespace Cinemachine
{
#if CINEMACHINE_PHYSICS
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that forces the LookAt
    /// point to the center of the screen, based on the Follow target's orientation,
    /// cancelling noise and other corrections.
    /// This is useful for third-person style aim cameras that want a dead-accurate
    /// aim at all times, even in the presence of positional or rotational noise.
    /// </summary>
    [AddComponentMenu("")] // Hide in menu
    [ExecuteAlways]
    [SaveDuringPlay]
    [DisallowMultipleComponent]
    public class Cinemachine3rdPersonAim : CinemachineExtension
    {
        /// <summary>Objects on these layers will be detected.</summary>
        [Header("Aim Target Detection")]
        [Tooltip("Objects on these layers will be detected")]
        public LayerMask AimCollisionFilter;

        /// <summary>Objects with this tag will be ignored.  
        /// It is a good idea to set this field to the target's tag.</summary>
        [TagField]
        [Tooltip("Objects with this tag will be ignored.  "
            + "It is a good idea to set this field to the target's tag")]
        public string IgnoreTag = string.Empty;

        /// <summary>How far to project the object detection ray.</summary>
        [Tooltip("How far to project the object detection ray")]
        public float AimDistance;

        /// <summary>This 2D object will be positioned in the game view over the raycast hit point, 
        /// if any, or will remain in the center of the screen if no hit point is 
        /// detected.  May be null, in which case no on-screen indicator will appear.</summary>
        [Tooltip("This 2D object will be positioned in the game view over the raycast hit point, if any, "
            + "or will remain in the center of the screen if no hit point is detected.  "
            + "May be null, in which case no on-screen indicator will appear")]
        public RectTransform AimTargetReticle;

        /// <summary>World space position of where the player would hit if a projectile were to 
        /// be fired from the player origin.  This may be different
        /// from state.ReferenceLookAt due to camera offset from player origin.</summary>
        public Vector3 AimTarget { get; private set; }

        private void OnValidate()
        {
            AimDistance = Mathf.Max(1, AimDistance);
        }

        private void Reset()
        {
            AimCollisionFilter = 1;
            IgnoreTag = string.Empty;
            AimDistance = 200.0f;
            AimTargetReticle = null;
        }

        /// <summary>Notification that this virtual camera is going live.</summary>
        /// <param name="fromCam">The camera being deactivated.  May be null.</param>
        /// <param name="worldUp">Default world Up, set by the CinemachineBrain</param>
        /// <param name="deltaTime">Delta time for time-based effects (ignore if less than or equal to 0)</param>
        /// <returns>True to request a vcam update of internal state</returns>
        public override bool OnTransitionFromCamera(
            ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime) 
        { 
            CinemachineCore.CameraUpdatedEvent.RemoveListener(DrawReticle);
            CinemachineCore.CameraUpdatedEvent.AddListener(DrawReticle);
            return false; 
        }

        void DrawReticle(CinemachineBrain brain)
        {
            if (!brain.IsLive(VirtualCamera) || brain.OutputCamera == null)
                CinemachineCore.CameraUpdatedEvent.RemoveListener(DrawReticle);
            else if (AimTargetReticle != null)
                AimTargetReticle.position = brain.OutputCamera.WorldToScreenPoint(AimTarget);
        }

        Vector3 ComputeLookAtPoint(Vector3 camPos, Transform player)
        {
            // We don't want to hit targets behind the player
            var aimDistance = AimDistance;
            var playerOrientation = player.rotation;
            var fwd = playerOrientation * Vector3.forward;
            var playerPos = Quaternion.Inverse(playerOrientation) * (player.position - camPos);
            if (playerPos.z > 0)
            {
                camPos += fwd * playerPos.z;
                aimDistance -= playerPos.z;
            }

            aimDistance = Mathf.Max(1, aimDistance);
            bool hasHit = RuntimeUtility.RaycastIgnoreTag(new Ray(camPos, fwd), 
                out RaycastHit hitInfo, aimDistance, AimCollisionFilter, IgnoreTag);
            return hasHit ? hitInfo.point : camPos + fwd * aimDistance;
        }
        
        Vector3 ComputeAimTarget(Vector3 cameraLookAt, Transform player)
        {
            // Adjust for actual player aim target (may be different due to offset)
            var playerPos = player.position;
            var dir = cameraLookAt - playerPos;
            if (RuntimeUtility.RaycastIgnoreTag(new Ray(playerPos, dir), 
                out RaycastHit hitInfo, dir.magnitude, AimCollisionFilter, IgnoreTag))
                return hitInfo.point;
            return cameraLookAt;
        }
        
        /// <summary>
        /// Sets the ReferenceLookAt to be the result of a raycast in the direction of camera forward.
        /// If an object is hit, point is placed there, else it is placed at AimDistance.
        /// </summary>
        /// <param name="vcam">The virtual camera being processed</param>
        /// <param name="stage">The current pipeline stage</param>
        /// <param name="state">The current virtual camera state</param>
        /// <param name="deltaTime">The current applicable deltaTime</param>
        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                // Raycast to establish what we're actually aiming at
                var player = vcam.Follow;
                if (player != null)
                {
                    state.ReferenceLookAt = ComputeLookAtPoint(state.CorrectedPosition, player);
                    AimTarget = ComputeAimTarget(state.ReferenceLookAt, player);
                }
            }
            if (stage == CinemachineCore.Stage.Finalize)
            {
                // Stabilize the LookAt point in the center of the screen
                var dir = state.ReferenceLookAt - state.FinalPosition;
                if (dir.sqrMagnitude > 0.01f)
                {
                    state.RawOrientation = Quaternion.LookRotation(dir, state.ReferenceUp);
                    state.OrientationCorrection = Quaternion.identity;
                }
            }
        }
    }
#endif
}
