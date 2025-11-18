#if !UNITY_2019_3_OR_NEWER
#define CINEMACHINE_PHYSICS
#endif

using UnityEngine;
using Cinemachine.Utility;

namespace Cinemachine
{
    /// <summary>
    /// Third-person follower, with complex pivoting: horizontal about the origin, 
    /// vertical about the shoulder.  
    /// </summary>
    [AddComponentMenu("")] // Don't display in add component menu
    [SaveDuringPlay]
    public class Cinemachine3rdPersonFollow : CinemachineComponentBase
    {
        /// <summary>How responsively the camera tracks the target.  Each axis (camera-local) 
        /// can have its own setting.  Value is the approximate time it takes the camera 
        /// to catch up to the target's new position.  Smaller values give a more rigid 
        /// effect, larger values give a squishier one.</summary>
        [Tooltip("How responsively the camera tracks the target.  Each axis (camera-local) "
           + "can have its own setting.  Value is the approximate time it takes the camera "
           + "to catch up to the target's new position.  Smaller values give a more "
           + "rigid effect, larger values give a squishier one")]
        public Vector3 Damping;

        /// <summary>Position of the shoulder pivot relative to the Follow target origin.  
        /// This offset is in target-local space.</summary>
        [Header("Rig")]
        [Tooltip("Position of the shoulder pivot relative to the Follow target origin.  "
            + "This offset is in target-local space")]
        public Vector3 ShoulderOffset;

        /// <summary>Vertical offset of the hand in relation to the shoulder.  
        /// Arm length will affect the follow target's screen position 
        /// when the camera rotates vertically.</summary>
        [Tooltip("Vertical offset of the hand in relation to the shoulder.  "
            + "Arm length will affect the follow target's screen position when "
            + "the camera rotates vertically")]
        public float VerticalArmLength;

        /// <summary>Specifies which shoulder (left, right, or in-between) the camera is on.</summary>
        [Tooltip("Specifies which shoulder (left, right, or in-between) the camera is on")]
        [Range(0, 1)]
        public float CameraSide;

        /// <summary>How far baehind the hand the camera will be placed.</summary>
        [Tooltip("How far baehind the hand the camera will be placed")]
        public float CameraDistance;

#if CINEMACHINE_PHYSICS
        /// <summary>Camera will avoid obstacles on these layers.</summary>
        [Header("Obstacles")]
        [Tooltip("Camera will avoid obstacles on these layers")]
        public LayerMask CameraCollisionFilter;

        /// <summary>
        /// Obstacles with this tag will be ignored.  It is a good idea 
        /// to set this field to the target's tag
        /// </summary>
        [TagField]
        [Tooltip("Obstacles with this tag will be ignored.  "
            + "It is a good idea to set this field to the target's tag")]
        public string IgnoreTag = string.Empty;

        /// <summary>
        /// Specifies how close the camera can get to obstacles
        /// </summary>
        [Tooltip("Specifies how close the camera can get to obstacles")]
        [Range(0, 1)]
        public float CameraRadius;
        
        /// <summary>
        /// How gradually the camera moves to correct for occlusions.  
        /// Higher numbers will move the camera more gradually.
        /// </summary>
        [Range(0, 10)]
        [Tooltip("How gradually the camera moves to correct for occlusions.  " +
            "Higher numbers will move the camera more gradually.")]
        public float DampingIntoCollision;

        /// <summary>
        /// How gradually the camera returns to its normal position after having been corrected by the built-in
        /// collision resolution system. Higher numbers will move the camera more gradually back to normal.
        /// </summary>
        [Range(0, 10)]
        [Tooltip("How gradually the camera returns to its normal position after having been corrected by the built-in " +
            "collision resolution system.  Higher numbers will move the camera more gradually back to normal.")]
        public float DampingFromCollision;
#endif

        // State info
        Vector3 m_PreviousFollowTargetPosition;
        Vector3 m_DampingCorrection; // this is in local rig space
#if CINEMACHINE_PHYSICS
        float m_CamPosCollisionCorrection;
#endif

        void OnValidate()
        {
            CameraSide = Mathf.Clamp(CameraSide, -1.0f, 1.0f);
            Damping.x = Mathf.Max(0, Damping.x);
            Damping.y = Mathf.Max(0, Damping.y);
            Damping.z = Mathf.Max(0, Damping.z);
#if CINEMACHINE_PHYSICS
            CameraRadius = Mathf.Max(0.001f, CameraRadius);
            DampingIntoCollision = Mathf.Max(0, DampingIntoCollision);
            DampingFromCollision = Mathf.Max(0, DampingFromCollision);
#endif
        }

        void Reset()
        {
            ShoulderOffset = new Vector3(0.5f, -0.4f, 0.0f);
            VerticalArmLength = 0.4f;
            CameraSide = 1.0f;
            CameraDistance = 2.0f;
            Damping = new Vector3(0.1f, 0.5f, 0.3f);
#if CINEMACHINE_PHYSICS
            CameraCollisionFilter = 0;
            CameraRadius = 0.2f;
            DampingIntoCollision = 0;
            DampingFromCollision = 2f;
#endif
        }

#if CINEMACHINE_PHYSICS
        void OnDestroy()
        {
            RuntimeUtility.DestroyScratchCollider();
        }
#endif
        
        /// <summary>True if component is enabled and has a Follow target defined</summary>
        public override bool IsValid => enabled && FollowTarget != null;

        /// <summary>Get the Cinemachine Pipeline stage that this component implements.
        /// Always returns the Aim stage</summary>
        public override CinemachineCore.Stage Stage { get { return CinemachineCore.Stage.Body; } }

#if CINEMACHINE_PHYSICS
        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime() 
        { 
            return Mathf.Max(
                Mathf.Max(DampingIntoCollision, DampingFromCollision), 
                Mathf.Max(Damping.x, Mathf.Max(Damping.y, Damping.z)));
        }
#endif

        /// <summary>Orients the camera to match the Follow target's orientation</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Elapsed time since last frame, for damping calculations.  
        /// If negative, previous state is reset.</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (IsValid)
            {
                if (!VirtualCamera.PreviousStateIsValid)
                    deltaTime = -1;
                PositionCamera(ref curState, deltaTime);
            }
        }

        /// <summary>This is called to notify the us that a target got warped,
        /// so that we can update its internal state to make the camera
        /// also warp seamlessy.</summary>
        /// <param name="target">The object that was warped</param>
        /// <param name="positionDelta">The amount the target's position changed</param>
        public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
        {
            base.OnTargetObjectWarped(target, positionDelta);
            if (target == FollowTarget)
            {
                m_PreviousFollowTargetPosition += positionDelta;
            }
        }
        
        void PositionCamera(ref CameraState curState, float deltaTime)
        {
            var up = curState.ReferenceUp;
            var targetPos = FollowTargetPosition;
            var targetRot = FollowTargetRotation;
            var targetForward = targetRot * Vector3.forward;
            var heading = GetHeading(targetRot, up);

            if (deltaTime < 0)
            {
                // No damping - reset damping state info
                m_DampingCorrection = Vector3.zero;
#if CINEMACHINE_PHYSICS
                m_CamPosCollisionCorrection = 0;
#endif
            }
            else
            {
                // Damping correction is applied to the shoulder offset - stretching the rig
                m_DampingCorrection += Quaternion.Inverse(heading) * (m_PreviousFollowTargetPosition - targetPos);
                m_DampingCorrection -= VirtualCamera.DetachedFollowTargetDamp(m_DampingCorrection, Damping, deltaTime);
            }

            m_PreviousFollowTargetPosition = targetPos;
            var root = targetPos;
            GetRawRigPositions(root, targetRot, heading, out _, out Vector3 hand);

            // Place the camera at the correct distance from the hand
            var camPos = hand - (targetForward * (CameraDistance - m_DampingCorrection.z));

#if CINEMACHINE_PHYSICS
            // Check if hand is colliding with something, if yes, then move the hand 
            // closer to the player. The radius is slightly enlarged, to avoid problems 
            // next to walls
            float dummy = 0;
            var collidedHand = ResolveCollisions(root, hand, -1, CameraRadius * 1.05f, ref dummy);
            camPos = ResolveCollisions(
                collidedHand, camPos, deltaTime, CameraRadius, ref m_CamPosCollisionCorrection);
#endif
            // Set state
            curState.RawPosition = camPos;
            curState.RawOrientation = targetRot; // not necessary, but left in to avoid breaking scenes that depend on this
        }
        
        /// <summary>
        /// Internal use only.  Public for the inspector gizmo
        /// </summary>
        /// <param name="root">Root of the rig.</param>
        /// <param name="shoulder">Shoulder of the rig.</param>
        /// <param name="hand">Hand of the rig.</param>
        public void GetRigPositions(out Vector3 root, out Vector3 shoulder, out Vector3 hand)
        {
            var up = VirtualCamera.State.ReferenceUp;
            var targetRot = FollowTargetRotation;
            var heading = GetHeading(targetRot, up);
            root = m_PreviousFollowTargetPosition;
            GetRawRigPositions(root, targetRot, heading, out shoulder, out hand);
#if CINEMACHINE_PHYSICS
            float dummy = 0;
            hand = ResolveCollisions(root, hand, -1, CameraRadius * 1.05f, ref dummy);
#endif
        }

        internal static Quaternion GetHeading(Quaternion targetRot, Vector3 up)
        {
            var targetForward = targetRot * Vector3.forward;
            var planeForward = Vector3.Cross(up, Vector3.Cross(targetForward.ProjectOntoPlane(up), up));
            if (planeForward.AlmostZero())
                planeForward = Vector3.Cross(targetRot * Vector3.right, up);
            return Quaternion.LookRotation(planeForward, up);
        }

        void GetRawRigPositions(
            Vector3 root, Quaternion targetRot, Quaternion heading, 
            out Vector3 shoulder, out Vector3 hand)
        {
            var shoulderOffset = ShoulderOffset;
            shoulderOffset.x = Mathf.Lerp(-shoulderOffset.x, shoulderOffset.x, CameraSide);
            shoulderOffset.x += m_DampingCorrection.x;
            shoulderOffset.y += m_DampingCorrection.y;
            shoulder = root + heading * shoulderOffset;
            hand = shoulder + targetRot * new Vector3(0, VerticalArmLength, 0);   
        }

#if CINEMACHINE_PHYSICS
        Vector3 ResolveCollisions(
            Vector3 root, Vector3 tip, float deltaTime, 
            float cameraRadius, ref float collisionCorrection)
        {
            if (CameraCollisionFilter.value == 0)
            {
                return tip;
            }
            
            var dir = tip - root;
            var len = dir.magnitude;
            dir /= len;

            var result = tip;
            float desiredCorrection = 0;

            if (RuntimeUtility.SphereCastIgnoreTag(
                root, cameraRadius, dir, out RaycastHit hitInfo, 
                len, CameraCollisionFilter, IgnoreTag))
            {
                var desiredResult = hitInfo.point + hitInfo.normal * cameraRadius;
                desiredCorrection = (desiredResult - tip).magnitude;
            }

            collisionCorrection += deltaTime < 0 ? desiredCorrection - collisionCorrection : Damper.Damp(
                desiredCorrection - collisionCorrection, 
                desiredCorrection > collisionCorrection ? DampingIntoCollision : DampingFromCollision, 
                deltaTime);

            // Apply the correction
            if (collisionCorrection > Epsilon)
                result -= dir * collisionCorrection;

            return result;
        }
#endif
    }
}
