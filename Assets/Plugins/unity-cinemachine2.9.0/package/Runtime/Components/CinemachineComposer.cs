using UnityEngine;
using System;
using Cinemachine.Utility;
using UnityEngine.Serialization;

namespace Cinemachine
{
    /// <summary>
    /// This is a CinemachineComponent in the Aim section of the component pipeline.
    /// Its job is to aim the camera at the vcam's LookAt target object, with
    /// configurable offsets, damping, and composition rules.
    ///
    /// The composer does not change the camera's position.  It will only pan and tilt the
    /// camera where it is, in order to get the desired framing.  To move the camera, you have
    /// to use the virtual camera's Body section.
    /// </summary>
    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    [AddComponentMenu("")] // Don't display in add component menu
    [SaveDuringPlay]
    public class CinemachineComposer : CinemachineComponentBase
    {
        /// <summary>Target offset from the object's center in LOCAL space which
        /// the Composer tracks. Use this to fine-tune the tracking target position
        /// when the desired area is not in the tracked object's center</summary>
        [Tooltip("Target offset from the target object's center in target-local space. Use this to "
            + "fine-tune the tracking target position when the desired area is not the tracked object's center.")]
        public Vector3 m_TrackedObjectOffset = Vector3.zero;

        /// <summary>This setting will instruct the composer to adjust its target offset based
        /// on the motion of the target.  The composer will look at a point where it estimates
        /// the target will be this many seconds into the future.  Note that this setting is sensitive
        /// to noisy animation, and can amplify the noise, resulting in undesirable camera jitter.
        /// If the camera jitters unacceptably when the target is in motion, turn down this setting,
        /// or animate the target more smoothly.</summary>
        [Space]
        [Tooltip("This setting will instruct the composer to adjust its target offset based on the motion "
            + "of the target.  The composer will look at a point where it estimates the target will be this "
            + "many seconds into the future.  Note that this setting is sensitive to noisy animation, and "
            + "can amplify the noise, resulting in undesirable camera jitter.  If the camera jitters "
            + "unacceptably when the target is in motion, turn down this setting, or animate the target more smoothly.")]
        [Range(0f, 1f)]
        public float m_LookaheadTime = 0;

        /// <summary>Controls the smoothness of the lookahead algorithm.  Larger values smooth out
        /// jittery predictions and also increase prediction lag</summary>
        [Tooltip("Controls the smoothness of the lookahead algorithm.  Larger values smooth "
            + "out jittery predictions and also increase prediction lag")]
        [Range(0, 30)]
        public float m_LookaheadSmoothing = 0;

        /// <summary>If checked, movement along the Y axis will be ignored for lookahead calculations</summary>
        [Tooltip("If checked, movement along the Y axis will be ignored for lookahead calculations")]
        public bool m_LookaheadIgnoreY;

        /// <summary>How aggressively the camera tries to follow the target in the screen-horizontal direction.
        /// Small numbers are more responsive, rapidly orienting the camera to keep the target in
        /// the dead zone. Larger numbers give a more heavy slowly responding camera.
        /// Using different vertical and horizontal settings can yield a wide range of camera behaviors.</summary>
        [Space]
        [Range(0f, 20)]
        [Tooltip("How aggressively the camera tries to follow the target in the screen-horizontal direction. "
            + "Small numbers are more responsive, rapidly orienting the camera to keep the target in "
            + "the dead zone. Larger numbers give a more heavy slowly responding camera. Using different "
            + "vertical and horizontal settings can yield a wide range of camera behaviors.")]
        public float m_HorizontalDamping = 0.5f;

        /// <summary>How aggressively the camera tries to follow the target in the screen-vertical direction.
        /// Small numbers are more responsive, rapidly orienting the camera to keep the target in
        /// the dead zone. Larger numbers give a more heavy slowly responding camera. Using different vertical
        /// and horizontal settings can yield a wide range of camera behaviors.</summary>
        [Range(0f, 20)]
        [Tooltip("How aggressively the camera tries to follow the target in the screen-vertical direction. "
            + "Small numbers are more responsive, rapidly orienting the camera to keep the target in "
            + "the dead zone. Larger numbers give a more heavy slowly responding camera. Using different "
            + "vertical and horizontal settings can yield a wide range of camera behaviors.")]
        public float m_VerticalDamping = 0.5f;

        /// <summary>Horizontal screen position for target. The camera will rotate to the position the tracked object here</summary>
        [Space]
        [Range(-0.5f, 1.5f)]
        [Tooltip("Horizontal screen position for target. The camera will rotate to position the tracked object here.")]
        public float m_ScreenX = 0.5f;

        /// <summary>Vertical screen position for target, The camera will rotate to to position the tracked object here</summary>
        [Range(-0.5f, 1.5f)]
        [Tooltip("Vertical screen position for target, The camera will rotate to position the tracked object here.")]
        public float m_ScreenY = 0.5f;

        /// <summary>Camera will not rotate horizontally if the target is within this range of the position</summary>
        [Range(0f, 2f)]
        [Tooltip("Camera will not rotate horizontally if the target is within this range of the position.")]
        public float m_DeadZoneWidth = 0f;

        /// <summary>Camera will not rotate vertically if the target is within this range of the position</summary>
        [Range(0f, 2f)]
        [Tooltip("Camera will not rotate vertically if the target is within this range of the position.")]
        public float m_DeadZoneHeight = 0f;

        /// <summary>When target is within this region, camera will gradually move to re-align
        /// towards the desired position, depending onm the damping speed</summary>
        [Range(0f, 2f)]
        [Tooltip("When target is within this region, camera will gradually rotate horizontally to re-align "
            + "towards the desired position, depending on the damping speed.")]
        public float m_SoftZoneWidth = 0.8f;

        /// <summary>When target is within this region, camera will gradually move to re-align
        /// towards the desired position, depending onm the damping speed</summary>
        [Range(0f, 2f)]
        [Tooltip("When target is within this region, camera will gradually rotate vertically to re-align "
            + "towards the desired position, depending on the damping speed.")]
        public float m_SoftZoneHeight = 0.8f;

        /// <summary>A non-zero bias will move the targt position away from the center of the soft zone</summary>
        [Range(-0.5f, 0.5f)]
        [Tooltip("A non-zero bias will move the target position horizontally away from the center of the soft zone.")]
        public float m_BiasX = 0f;

        /// <summary>A non-zero bias will move the targt position away from the center of the soft zone</summary>
        [Range(-0.5f, 0.5f)]
        [Tooltip("A non-zero bias will move the target position vertically away from the center of the soft zone.")]
        public float m_BiasY = 0f;

        /// <summary>Force target to center of screen when this camera activates.  
        /// If false, will clamp target to the edges of the dead zone</summary>
        [Tooltip("Force target to center of screen when this camera activates.  If false, will "
            + "clamp target to the edges of the dead zone")]
        public bool m_CenterOnActivate = true;

        /// <summary>True if component is enabled and has a LookAt defined</summary>
        public override bool IsValid { get { return enabled && LookAtTarget != null; } }

        /// <summary>Get the Cinemachine Pipeline stage that this component implements.
        /// Always returns the Aim stage</summary>
        public override CinemachineCore.Stage Stage { get { return CinemachineCore.Stage.Aim; } }

        /// <summary>Internal API for inspector</summary>
        public Vector3 TrackedPoint { get; private set; }

        /// <summary>Apply the target offsets to the target location.
        /// Also set the TrackedPoint property, taking lookahead into account.</summary>
        /// <param name="lookAt">The unoffset LookAt point</param>
        /// <param name="up">Currest effective world up</param>
        /// <param name="deltaTime">Current effective deltaTime</param>
        /// <returns>The LookAt point with the offset applied</returns>
        protected virtual Vector3 GetLookAtPointAndSetTrackedPoint(
            Vector3 lookAt, Vector3 up, float deltaTime)
        {
            Vector3 pos = lookAt;
            if (LookAtTarget != null)
                pos += LookAtTargetRotation * m_TrackedObjectOffset;

            if (m_LookaheadTime < Epsilon)
                TrackedPoint = pos;
            else
            {
                var resetLookahead = VirtualCamera.LookAtTargetChanged || !VirtualCamera.PreviousStateIsValid;
                m_Predictor.Smoothing = m_LookaheadSmoothing;
                m_Predictor.AddPosition(pos, resetLookahead ? -1 : deltaTime, m_LookaheadTime);
                var delta = m_Predictor.PredictPositionDelta(m_LookaheadTime);
                if (m_LookaheadIgnoreY)
                    delta = delta.ProjectOntoPlane(up);
                TrackedPoint = pos + delta;
            }
            return pos;
        }

        /// <summary>State information for damping</summary>
        Vector3 m_CameraPosPrevFrame = Vector3.zero;
        Vector3 m_LookAtPrevFrame = Vector3.zero;
        Vector2 m_ScreenOffsetPrevFrame = Vector2.zero;
        Quaternion m_CameraOrientationPrevFrame = Quaternion.identity;
        internal PositionPredictor m_Predictor = new PositionPredictor();

        /// <summary>This is called to notify the us that a target got warped,
        /// so that we can update its internal state to make the camera
        /// also warp seamlessy.</summary>
        /// <param name="target">The object that was warped</param>
        /// <param name="positionDelta">The amount the target's position changed</param>
        public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
        {
            base.OnTargetObjectWarped(target, positionDelta);
            if (target == LookAtTarget)
            {
                m_CameraPosPrevFrame += positionDelta;
                m_LookAtPrevFrame += positionDelta;
                m_Predictor.ApplyTransformDelta(positionDelta);
            }
        }

        /// <summary>
        /// Force the virtual camera to assume a given position and orientation
        /// </summary>
        /// <param name="pos">Worldspace pposition to take</param>
        /// <param name="rot">Worldspace orientation to take</param>
        public override void ForceCameraPosition(Vector3 pos, Quaternion rot)
        {
            base.ForceCameraPosition(pos, rot);
            m_CameraPosPrevFrame = pos;
            m_CameraOrientationPrevFrame = rot;
        }
        
        /// <summary>
        /// Report maximum damping time needed for this component.
        /// </summary>
        /// <returns>Highest damping setting in this component</returns>
        public override float GetMaxDampTime() 
        { 
            return Mathf.Max(m_HorizontalDamping, m_VerticalDamping); 
        }

        /// <summary>Sets the state's ReferenceLookAt, applying the offset.</summary>
        /// <param name="curState">Input state that must be mutated</param>
        /// <param name="deltaTime">Current effective deltaTime</param>
        public override void PrePipelineMutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (IsValid && curState.HasLookAt)
                curState.ReferenceLookAt = GetLookAtPointAndSetTrackedPoint(
                    curState.ReferenceLookAt, curState.ReferenceUp, deltaTime);
        }

        /// <summary>Applies the composer rules and orients the camera accordingly</summary>
        /// <param name="curState">The current camera state</param>
        /// <param name="deltaTime">Used for calculating damping.  If less than
        /// zero, then target will snap to the center of the dead zone.</param>
        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (!IsValid || !curState.HasLookAt)
                return;

            // Correct the tracked point in the event that it's behind the camera
            // while the real target is in front
            if (!(TrackedPoint - curState.ReferenceLookAt).AlmostZero())
            {
                Vector3 mid = Vector3.Lerp(curState.CorrectedPosition, curState.ReferenceLookAt, 0.5f);
                Vector3 toLookAt = curState.ReferenceLookAt - mid;
                Vector3 toTracked = TrackedPoint - mid;
                if (Vector3.Dot(toLookAt, toTracked) < 0)
                {
                    float t = Vector3.Distance(curState.ReferenceLookAt, mid)
                        / Vector3.Distance(curState.ReferenceLookAt, TrackedPoint);
                    TrackedPoint = Vector3.Lerp(curState.ReferenceLookAt, TrackedPoint, t);
                }
            }

            float targetDistance = (TrackedPoint - curState.CorrectedPosition).magnitude;
            if (targetDistance < Epsilon)
            {
                if (deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
                    curState.RawOrientation = m_CameraOrientationPrevFrame;
                return;  // navel-gazing, get outa here
            }

            // Expensive FOV calculations
            mCache.UpdateCache(curState.Lens, SoftGuideRect, HardGuideRect, targetDistance);

            Quaternion rigOrientation = curState.RawOrientation;
            if (deltaTime < 0 || !VirtualCamera.PreviousStateIsValid)
            {
                // No damping, just snap to central bounds, skipping the soft zone
                rigOrientation = Quaternion.LookRotation(
                    rigOrientation * Vector3.forward, curState.ReferenceUp);
                Rect rect = mCache.mFovSoftGuideRect;
                if (m_CenterOnActivate)
                    rect = new Rect(rect.center, Vector2.zero); // Force to center
                RotateToScreenBounds(
                    ref curState, rect, curState.ReferenceLookAt,
                    ref rigOrientation, mCache.mFov, mCache.mFovH, -1);
            }
            else
            {
                // Start with previous frame's orientation (but with current up)
                Vector3 dir = m_LookAtPrevFrame - m_CameraPosPrevFrame;
                if (dir.AlmostZero())
                    rigOrientation = Quaternion.LookRotation(
                        m_CameraOrientationPrevFrame * Vector3.forward, curState.ReferenceUp);
                else
                {
                    dir = Quaternion.Euler(curState.PositionDampingBypass) * dir;
                    rigOrientation = Quaternion.LookRotation(dir, curState.ReferenceUp);
                    rigOrientation = rigOrientation.ApplyCameraRotation(
                        -m_ScreenOffsetPrevFrame, curState.ReferenceUp);
                }

                // Move target through the soft zone, with damping
                RotateToScreenBounds(
                    ref curState, mCache.mFovSoftGuideRect, TrackedPoint,
                    ref rigOrientation, mCache.mFov, mCache.mFovH, deltaTime);

                // Force the actual target (not the lookahead one) into the hard bounds, no damping
                if (deltaTime < 0 || VirtualCamera.LookAtTargetAttachment > 1 - Epsilon)
                    RotateToScreenBounds(
                        ref curState, mCache.mFovHardGuideRect, curState.ReferenceLookAt,
                        ref rigOrientation, mCache.mFov, mCache.mFovH, -1);
            }

            m_CameraPosPrevFrame = curState.CorrectedPosition;
            m_LookAtPrevFrame = TrackedPoint;
            m_CameraOrientationPrevFrame = UnityQuaternionExtensions.Normalized(rigOrientation);
            m_ScreenOffsetPrevFrame = m_CameraOrientationPrevFrame.GetCameraRotationToTarget(
                m_LookAtPrevFrame - curState.CorrectedPosition, curState.ReferenceUp);

            curState.RawOrientation = m_CameraOrientationPrevFrame;
        }

        /// <summary>Internal API for the inspector editor</summary>
        internal Rect SoftGuideRect
        {
            get
            {
                return new Rect(
                    m_ScreenX - m_DeadZoneWidth / 2, m_ScreenY - m_DeadZoneHeight / 2,
                    m_DeadZoneWidth, m_DeadZoneHeight);
            }
            set
            {
                m_DeadZoneWidth = Mathf.Clamp(value.width, 0, 2);
                m_DeadZoneHeight = Mathf.Clamp(value.height, 0, 2);
                m_ScreenX = Mathf.Clamp(value.x + m_DeadZoneWidth / 2, -0.5f,  1.5f);
                m_ScreenY = Mathf.Clamp(value.y + m_DeadZoneHeight / 2, -0.5f,  1.5f);
                m_SoftZoneWidth = Mathf.Max(m_SoftZoneWidth, m_DeadZoneWidth);
                m_SoftZoneHeight = Mathf.Max(m_SoftZoneHeight, m_DeadZoneHeight);
            }
        }

        /// <summary>Internal API for the inspector editor</summary>
        internal Rect HardGuideRect
        {
            get
            {
                Rect r = new Rect(
                        m_ScreenX - m_SoftZoneWidth / 2, m_ScreenY - m_SoftZoneHeight / 2,
                        m_SoftZoneWidth, m_SoftZoneHeight);
                r.position += new Vector2(
                        m_BiasX * (m_SoftZoneWidth - m_DeadZoneWidth),
                        m_BiasY * (m_SoftZoneHeight - m_DeadZoneHeight));
                return r;
            }
            set
            {
                m_SoftZoneWidth = Mathf.Clamp(value.width, 0, 2f);
                m_SoftZoneHeight = Mathf.Clamp(value.height, 0, 2f);
                m_DeadZoneWidth = Mathf.Min(m_DeadZoneWidth, m_SoftZoneWidth);
                m_DeadZoneHeight = Mathf.Min(m_DeadZoneHeight, m_SoftZoneHeight);

                Vector2 center = value.center;
                Vector2 bias = center - new Vector2(m_ScreenX, m_ScreenY);
                float biasWidth = Mathf.Max(0, m_SoftZoneWidth - m_DeadZoneWidth);
                float biasHeight = Mathf.Max(0, m_SoftZoneHeight - m_DeadZoneHeight);
                m_BiasX = biasWidth < Epsilon ? 0 : Mathf.Clamp(bias.x / biasWidth, -0.5f, 0.5f);
                m_BiasY = biasHeight < Epsilon ? 0 : Mathf.Clamp(bias.y / biasHeight, -0.5f, 0.5f);
            }
        }

        // Cache for some expensive calculations
        struct FovCache
        {
            public Rect mFovSoftGuideRect;
            public Rect mFovHardGuideRect;
            public float mFovH;
            public float mFov;

            float mOrthoSizeOverDistance;
            float mAspect;
            Rect mSoftGuideRect;
            Rect mHardGuideRect;

            public void UpdateCache(
                LensSettings lens, Rect softGuide, Rect hardGuide, float targetDistance)
            {
                bool recalculate = mAspect != lens.Aspect
                    || softGuide != mSoftGuideRect || hardGuide != mHardGuideRect;
                if (lens.Orthographic)
                {
                    float orthoOverDistance = Mathf.Abs(lens.OrthographicSize / targetDistance);
                    if (mOrthoSizeOverDistance == 0
                        || Mathf.Abs(orthoOverDistance - mOrthoSizeOverDistance) / mOrthoSizeOverDistance
                            > mOrthoSizeOverDistance * 0.01f)
                        recalculate = true;
                    if (recalculate)
                    {
                        // Calculate effective fov - fake it for ortho based on target distance
                        mFov = Mathf.Rad2Deg * 2 * Mathf.Atan(orthoOverDistance);
                        mFovH = Mathf.Rad2Deg * 2 * Mathf.Atan(lens.Aspect * orthoOverDistance);
                        mOrthoSizeOverDistance = orthoOverDistance;
                    }
                }
                else
                {
                    var verticalFOV = lens.FieldOfView;
                    if (mFov != verticalFOV)
                        recalculate = true;
                    if (recalculate)
                    {
                        mFov = verticalFOV;
                        double radHFOV = 2 * Math.Atan(Math.Tan(mFov * Mathf.Deg2Rad / 2) * lens.Aspect);
                        mFovH = (float)(Mathf.Rad2Deg * radHFOV);
                        mOrthoSizeOverDistance = 0;
                    }
                }
                if (recalculate)
                {
                    mFovSoftGuideRect = ScreenToFOV(softGuide, mFov, mFovH, lens.Aspect);
                    mSoftGuideRect = softGuide;
                    mFovHardGuideRect = ScreenToFOV(hardGuide, mFov, mFovH, lens.Aspect);
                    mHardGuideRect = hardGuide;
                    mAspect = lens.Aspect;
                }
            }

            // Convert from screen coords to normalized FOV angular coords
            private Rect ScreenToFOV(Rect rScreen, float fov, float fovH, float aspect)
            {
                Rect r = new Rect(rScreen);
                Matrix4x4 persp = Matrix4x4.Perspective(fov, aspect, 0.0001f, 2f).inverse;

                Vector3 p = persp.MultiplyPoint(new Vector3(0, (r.yMin * 2f) - 1f, 0.5f)); p.z = -p.z;
                float angle = UnityVectorExtensions.SignedAngle(Vector3.forward, p, Vector3.left);
                r.yMin = ((fov / 2) + angle) / fov;

                p = persp.MultiplyPoint(new Vector3(0, (r.yMax * 2f) - 1f, 0.5f)); p.z = -p.z;
                angle = UnityVectorExtensions.SignedAngle(Vector3.forward, p, Vector3.left);
                r.yMax = ((fov / 2) + angle) / fov;

                p = persp.MultiplyPoint(new Vector3((r.xMin * 2f) - 1f, 0, 0.5f));  p.z = -p.z;
                angle = UnityVectorExtensions.SignedAngle(Vector3.forward, p, Vector3.up);
                r.xMin = ((fovH / 2) + angle) / fovH;

                p = persp.MultiplyPoint(new Vector3((r.xMax * 2f) - 1f, 0, 0.5f));  p.z = -p.z;
                angle = UnityVectorExtensions.SignedAngle(Vector3.forward, p, Vector3.up);
                r.xMax = ((fovH / 2) + angle) / fovH;
                return r;
            }
        }
        FovCache mCache;


        /// <summary>
        /// Adjust the rigOrientation to put the camera within the screen bounds.
        /// If deltaTime >= 0 then damping will be applied.
        /// Assumes that currentOrientation fwd is such that input rigOrientation's
        /// local up is NEVER NEVER NEVER pointing downwards, relative to
        /// state.ReferenceUp.  If this condition is violated
        /// then you will see crazy spinning.  That's the symptom.
        /// </summary>
        private void RotateToScreenBounds(
            ref CameraState state, Rect screenRect, Vector3 trackedPoint,
            ref Quaternion rigOrientation, float fov, float fovH, float deltaTime)
        {
            Vector3 targetDir = trackedPoint - state.CorrectedPosition;
            Vector2 rotToRect = rigOrientation.GetCameraRotationToTarget(targetDir, state.ReferenceUp);

            // Bring it to the edge of screenRect, if outside.  Leave it alone if inside.
            ClampVerticalBounds(ref screenRect, targetDir, state.ReferenceUp, fov);
            float min = (screenRect.yMin - 0.5f) * fov;
            float max = (screenRect.yMax - 0.5f) * fov;
            if (rotToRect.x < min)
                rotToRect.x -= min;
            else if (rotToRect.x > max)
                rotToRect.x -= max;
            else
                rotToRect.x = 0;

            min = (screenRect.xMin - 0.5f) * fovH;
            max = (screenRect.xMax - 0.5f) * fovH;
            if (rotToRect.y < min)
                rotToRect.y -= min;
            else if (rotToRect.y > max)
                rotToRect.y -= max;
            else
                rotToRect.y = 0;

            // Apply damping
            if (deltaTime >= 0 && VirtualCamera.PreviousStateIsValid)
            {
                rotToRect.x = VirtualCamera.DetachedLookAtTargetDamp(
                    rotToRect.x, m_VerticalDamping, deltaTime);
                rotToRect.y = VirtualCamera.DetachedLookAtTargetDamp(
                    rotToRect.y, m_HorizontalDamping, deltaTime);
            }

            // Rotate
            rigOrientation = rigOrientation.ApplyCameraRotation(rotToRect, state.ReferenceUp);
        }

        /// <summary>
        /// Prevent upside-down camera situation.  This can happen if we have a high
        /// camera pitch combined with composer settings that cause the camera to tilt
        /// beyond the vertical in order to produce the desired framing.  We prevent this by
        /// clamping the composer's vertical settings so that this situation can't happen.
        /// </summary>
        private bool ClampVerticalBounds(ref Rect r, Vector3 dir, Vector3 up, float fov)
        {
            float angle = UnityVectorExtensions.Angle(dir, up);
            float halfFov = (fov / 2f) + 1; // give it a little extra to accommodate precision errors
            if (angle < halfFov)
            {
                // looking up
                float maxY = 1f - (halfFov - angle) / fov;
                if (r.yMax > maxY)
                {
                    r.yMin = Mathf.Min(r.yMin, maxY);
                    r.yMax = Mathf.Min(r.yMax, maxY);
                    return true;
                }
            }
            if (angle > (180 - halfFov))
            {
                // looking down
                float minY = (angle - (180 - halfFov)) / fov;
                if (minY > r.yMin)
                {
                    r.yMin = Mathf.Max(r.yMin, minY);
                    r.yMax = Mathf.Max(r.yMax, minY);
                    return true;
                }
            }
            return false;
        }
    }
}
