using UnityEngine;
using Cinemachine.Utility;

namespace Cinemachine
{
    /// <summary>
    /// This is a CinemachineComponent in the Aim section of the component pipeline.
    /// Its job is to aim the camera at a target object, with configurable offsets, damping,
    /// and composition rules.
    ///
    /// In addition, if the target is a ICinemachineTargetGroup, the behaviour
    /// will adjust the FOV and the camera distance to ensure that the entire group of targets
    /// is framed properly.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [AddComponentMenu("")] // Don't display in add component menu
    [SaveDuringPlay]
    public class CinemachineGroupComposer : CinemachineComposer
    {
        /// <summary>How much of the screen to fill with the bounding box of the targets.</summary>
        [Space]
        [Tooltip("The bounding box of the targets should occupy this amount of the screen space.  1 means fill the whole screen.  0.5 means fill half the screen, etc.")]
        public float m_GroupFramingSize = 0.8f;

        /// <summary>What screen dimensions to consider when framing</summary>
        [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
        public enum FramingMode
        {
            /// <summary>Consider only the horizontal dimension.  Vertical framing is ignored.</summary>
            Horizontal,
            /// <summary>Consider only the vertical dimension.  Horizontal framing is ignored.</summary>
            Vertical,
            /// <summary>The larger of the horizontal and vertical dimensions will dominate, to get the best fit.</summary>
            HorizontalAndVertical
        };

        /// <summary>What screen dimensions to consider when framing</summary>
        [Tooltip("What screen dimensions to consider when framing.  Can be Horizontal, Vertical, or both")]
        public FramingMode m_FramingMode = FramingMode.HorizontalAndVertical;

        /// <summary>How aggressively the camera tries to frame the group.
        /// Small numbers are more responsive</summary>
        [Range(0, 20)]
        [Tooltip("How aggressively the camera tries to frame the group. Small numbers are more responsive, rapidly adjusting the camera to keep the group in the frame.  Larger numbers give a more heavy slowly responding camera.")]
        public float m_FrameDamping = 2f;

        /// <summary>How to adjust the camera to get the desired framing</summary>
        public enum AdjustmentMode
        {
            /// <summary>Do not move the camera, only adjust the FOV.</summary>
            ZoomOnly,
            /// <summary>Just move the camera, don't change the FOV.</summary>
            DollyOnly,
            /// <summary>Move the camera as much as permitted by the ranges, then
            /// adjust the FOV if necessary to make the shot.</summary>
            DollyThenZoom
        };

        /// <summary>How to adjust the camera to get the desired framing</summary>
        [Tooltip("How to adjust the camera to get the desired framing.  You can zoom, dolly in/out, or do both.")]
        public AdjustmentMode m_AdjustmentMode = AdjustmentMode.ZoomOnly;

        /// <summary>How much closer to the target can the camera go?</summary>
        [Tooltip("The maximum distance toward the target that this behaviour is allowed to move the camera.")]
        public float m_MaxDollyIn = 5000f;

        /// <summary>How much farther from the target can the camera go?</summary>
        [Tooltip("The maximum distance away the target that this behaviour is allowed to move the camera.")]
        public float m_MaxDollyOut = 5000f;

        /// <summary>Set this to limit how close to the target the camera can get</summary>
        [Tooltip("Set this to limit how close to the target the camera can get.")]
        public float m_MinimumDistance = 1;

        /// <summary>Set this to limit how far from the taregt the camera can get</summary>
        [Tooltip("Set this to limit how far from the target the camera can get.")]
        public float m_MaximumDistance = 5000f;

        /// <summary>If adjusting FOV, will not set the FOV lower than this</summary>
        [Range(1, 179)]
        [Tooltip("If adjusting FOV, will not set the FOV lower than this.")]
        public float m_MinimumFOV = 3;

        /// <summary>If adjusting FOV, will not set the FOV higher than this</summary>
        [Range(1, 179)]
        [Tooltip("If adjusting FOV, will not set the FOV higher than this.")]
        public float m_MaximumFOV = 60;

        /// <summary>If adjusting Orthographic Size, will not set it lower than this</summary>
        [Tooltip("If adjusting Orthographic Size, will not set it lower than this.")]
        public float m_MinimumOrthoSize = 1;

        /// <summary>If adjusting Orthographic Size, will not set it higher than this</summary>
        [Tooltip("If adjusting Orthographic Size, will not set it higher than this.")]
        public float m_MaximumOrthoSize = 5000;

        private void OnValidate()
        {
            m_GroupFramingSize = Mathf.Max(0.001f, m_GroupFramingSize);
            m_MaxDollyIn = Mathf.Max(0, m_MaxDollyIn);
            m_MaxDollyOut = Mathf.Max(0, m_MaxDollyOut);
            m_MinimumDistance = Mathf.Max(0, m_MinimumDistance);
            m_MaximumDistance = Mathf.Max(m_MinimumDistance, m_MaximumDistance);
            m_MinimumFOV = Mathf.Max(1, m_MinimumFOV);
            m_MaximumFOV = Mathf.Clamp(m_MaximumFOV, m_MinimumFOV, 179);
            m_MinimumOrthoSize = Mathf.Max(0.01f, m_MinimumOrthoSize);
            m_MaximumOrthoSize = Mathf.Max(m_MinimumOrthoSize, m_MaximumOrthoSize);
        }

        // State for damping
        float m_prevFramingDistance;
        float m_prevFOV;

        /// <summary>For editor visulaization of the calculated bounding box of the group</summary>
        public Bounds LastBounds { get; private set; }

        /// <summary>For editor visualization of the calculated bounding box of the group</summary>
        public Matrix4x4 LastBoundsMatrix { get; private set; }

        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime() 
        { 
            return Mathf.Max(base.GetMaxDampTime(), m_FrameDamping); 
        }

        /// <summary>Applies the composer rules and orients the camera accordingly</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Used for calculating damping.  If less than
        /// zero, then target will snap to the center of the dead zone.</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            // Can't do anything without a group to look at
            ICinemachineTargetGroup group = AbstractLookAtTargetGroup;
            if (group == null)
            {
                base.MutateCameraState(ref curState, deltaTime);
                return;
            }

            if (!IsValid || !curState.HasLookAt)
            {
                m_prevFramingDistance = 0;
                m_prevFOV = 0;
                return;
            }

            bool isOrthographic = curState.Lens.Orthographic;
            bool canMoveCamera = !isOrthographic && m_AdjustmentMode != AdjustmentMode.ZoomOnly;

            // Get the bounding box from camera's POV in view space
            Vector3 up = curState.ReferenceUp;
            var cameraPos = curState.RawPosition;
            BoundingSphere s = group.Sphere;
            Vector3 groupCenter = s.position;
            Vector3 fwd = groupCenter - cameraPos;
            float d = fwd.magnitude;
            if (d < Epsilon)
                return;  // navel-gazing, get outa here

            // Approximate looking at the group center
            fwd /= d;
            LastBoundsMatrix = Matrix4x4.TRS(
                cameraPos, Quaternion.LookRotation(fwd, up), Vector3.one);

            // Correction for the actual center
            Bounds b;
            if (isOrthographic)
            {
                b = group.GetViewSpaceBoundingBox(LastBoundsMatrix);
                groupCenter = LastBoundsMatrix.MultiplyPoint3x4(b.center);
                fwd = (groupCenter - cameraPos).normalized;
                LastBoundsMatrix = Matrix4x4.TRS(cameraPos, Quaternion.LookRotation(fwd, up), Vector3.one);
                b = group.GetViewSpaceBoundingBox(LastBoundsMatrix);
                LastBounds = b;
            }
            else
            {
                b = GetScreenSpaceGroupBoundingBox(group, LastBoundsMatrix, out fwd);
                LastBoundsMatrix = Matrix4x4.TRS(cameraPos, Quaternion.LookRotation(fwd, up), Vector3.one);
                LastBounds = b;
                groupCenter = cameraPos + fwd * b.center.z;
            }

            // Adjust bounds for framing size, and get target height
            float boundsDepth = b.extents.z;
            float targetHeight = GetTargetHeight(b.size / m_GroupFramingSize);

            if (isOrthographic)
            {
                targetHeight = Mathf.Clamp(targetHeight / 2, m_MinimumOrthoSize, m_MaximumOrthoSize);

                // ApplyDamping
                if (deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
                    targetHeight = m_prevFOV + VirtualCamera.DetachedLookAtTargetDamp(
                        targetHeight - m_prevFOV, m_FrameDamping, deltaTime);
                m_prevFOV = targetHeight;

                LensSettings lens = curState.Lens;
                lens.OrthographicSize = Mathf.Clamp(targetHeight, m_MinimumOrthoSize, m_MaximumOrthoSize);
                curState.Lens = lens;
            }
            else
            {
                // Adjust height for perspective - we want the height at the near surface
                float z = b.center.z;
                if (z > boundsDepth)
                    targetHeight = Mathf.Lerp(0, targetHeight, (z - boundsDepth) / z);

                // Move the camera
                if (canMoveCamera)
                {
                    // What distance from near edge would be needed to get the adjusted
                    // target height, at the current FOV
                    float targetDistance = boundsDepth
                        + targetHeight / (2f * Mathf.Tan(curState.Lens.FieldOfView * Mathf.Deg2Rad / 2f));

                    // Clamp to respect min/max distance settings to the near surface of the bounds
                    targetDistance = Mathf.Clamp(
                        targetDistance, boundsDepth + m_MinimumDistance, boundsDepth + m_MaximumDistance);

                    // Clamp to respect min/max camera movement
                    float targetDelta = targetDistance - Vector3.Distance(curState.RawPosition, groupCenter);
                    targetDelta = Mathf.Clamp(targetDelta, -m_MaxDollyIn, m_MaxDollyOut);

                    // ApplyDamping
                    if (deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
                    {
                        float delta = targetDelta - m_prevFramingDistance;
                        delta = VirtualCamera.DetachedLookAtTargetDamp(delta, m_FrameDamping, deltaTime);
                        targetDelta = m_prevFramingDistance + delta;
                    }
                    m_prevFramingDistance = targetDelta;
                    curState.PositionCorrection -= fwd * targetDelta;
                    cameraPos -= fwd * targetDelta;
                }

                // Apply zoom
                if (m_AdjustmentMode != AdjustmentMode.DollyOnly)
                {
                    float nearBoundsDistance = (groupCenter - cameraPos).magnitude - boundsDepth;
                    float targetFOV = 179;
                    if (nearBoundsDistance > Epsilon)
                        targetFOV = 2f * Mathf.Atan(targetHeight / (2 * nearBoundsDistance)) * Mathf.Rad2Deg;
                    targetFOV = Mathf.Clamp(targetFOV, m_MinimumFOV, m_MaximumFOV);

                    // ApplyDamping
                    if (deltaTime >= 0 && m_prevFOV != 0 && VirtualCamera.PreviousStateIsValid)
                        targetFOV = m_prevFOV + VirtualCamera.DetachedLookAtTargetDamp(
                            targetFOV - m_prevFOV, m_FrameDamping, deltaTime);
                    m_prevFOV = targetFOV;

                    LensSettings lens = curState.Lens;
                    lens.FieldOfView = targetFOV;
                    curState.Lens = lens;
                }
            }
            // Now compose normally
            curState.ReferenceLookAt = GetLookAtPointAndSetTrackedPoint(
                groupCenter, curState.ReferenceUp, deltaTime);
            base.MutateCameraState(ref curState, deltaTime);
        }

        float GetTargetHeight(Vector2 boundsSize)
        {
            switch (m_FramingMode)
            {
                case FramingMode.Horizontal:
                    return Mathf.Max(Epsilon, boundsSize.x ) / VcamState.Lens.Aspect;
                case FramingMode.Vertical:
                    return Mathf.Max(Epsilon, boundsSize.y);
                default:
                case FramingMode.HorizontalAndVertical:
                    return Mathf.Max(
                        Mathf.Max(Epsilon, boundsSize.x) / VcamState.Lens.Aspect,
                        Mathf.Max(Epsilon, boundsSize.y));
            }
        }

        /// <param name="observer">Point of view</param>
        /// <param name="newFwd">New forward direction to use when interpreting the return value</param>
        /// <returns>Bounding box in a slightly rotated version of observer, as specified by newFwd</returns>
        static Bounds GetScreenSpaceGroupBoundingBox(
            ICinemachineTargetGroup group, Matrix4x4 observer, out Vector3 newFwd)
        {
            group.GetViewSpaceAngularBounds(observer, out var minAngles, out var maxAngles, out var zRange);
            var shift = (minAngles + maxAngles) / 2;

            newFwd = Quaternion.identity.ApplyCameraRotation(new Vector2(-shift.x, shift.y), Vector3.up) * Vector3.forward;
            newFwd = observer.MultiplyVector(newFwd);

            // For width and height (in camera space) of the bounding box, we use the values at the center of the box.
            // This is an arbitrary choice.  The gizmo drawer will take this into account when displaying
            // the frustum bounds of the group
            var d = zRange.y + zRange.x;
            var angles = Vector2.Min(maxAngles - shift, new Vector2(89.5f, 89.5f)) * Mathf.Deg2Rad;
            return new Bounds(
                new Vector3(0, 0, d/2),
                new Vector3(Mathf.Tan(angles.y) * d, Mathf.Tan(angles.x) * d, zRange.y - zRange.x));
        }
    }
}
