using UnityEngine;

namespace RootMotion.FinalIK
{
    public partial class IKSolverVR : IKSolver
    {
        public partial class Locomotion
        {
            [Tooltip("Start moving (horizontal distance to HMD + HMD velocity) threshold.")]
            /// <summary>
            /// Start moving (horizontal distance to HMD + HMD velocity) threshold.
            /// </summary>
            [ShowIf("mode", Mode.Animated)]
            public float moveThreshold = 0.3f;

            // ANIMATION
            [ShowLargeHeaderIf("Animation", "mode", Mode.Animated)] [SerializeField] byte animationHeader;

            [Tooltip("Minimum locomotion animation speed.")]
            /// <summary>
            /// Minimum locomotion animation speed.
            /// </summary>
            [ShowRangeIf(0.1f, 1f, "mode", Mode.Animated)]
            public float minAnimationSpeed = 0.2f;

            [Tooltip("Maximum locomotion animation speed.")]
            /// <summary>
            /// Maximum locomotion animation speed.
            /// </summary>
            [ShowRangeIf(1f, 10f, "mode", Mode.Animated)]
            public float maxAnimationSpeed = 3f;

            [Tooltip("Smoothing time for Vector3.SmoothDamping 'VRIK_Horizontal' and 'VRIK_Vertical' parameters. Larger values make animation smoother, but less responsive.")]
            /// <summary>
            /// Smoothing time for Vector3.SmoothDamping 'VRIK_Horizontal' and 'VRIK_Vertical' parameters. Larger values make animation smoother, but less responsive.
            /// </summary>
            [ShowRangeIf(0.05f, 0.2f, "mode", Mode.Animated)]
            public float animationSmoothTime = 0.1f;

            [ShowLargeHeaderIf("Root Position", "mode", Mode.Animated)] [SerializeField] byte rootPositionHeader;

            [Tooltip("X and Z standing offset from the horizontal position of the HMD.")]
            /// <summary>
            /// X and Z standing offset from the horizontal position of the HMD.
            /// </summary>
            [ShowIf("mode", Mode.Animated)]
            public Vector2 standOffset;

            [Tooltip("Lerp root towards the horizontal position of the HMD with this speed while moving.")]
            /// <summary>
            /// Lerp root towards the horizontal position of the HMD with this speed while moving.
            /// </summary>
            [ShowRangeIf(0f, 50f, "mode", Mode.Animated)]
            public float rootLerpSpeedWhileMoving = 30f;

            [Tooltip("Lerp root towards the horizontal position of the HMD with this speed while in transition from locomotion to idle state.")]
            /// <summary>
            /// Lerp root towards the horizontal position of the HMD with this speed while in transition from locomotion to idle state.
            /// </summary>
            [ShowRangeIf(0f, 50f, "mode", Mode.Animated)]
            public float rootLerpSpeedWhileStopping = 10f;

            [Tooltip("Lerp root towards the horizontal position of the HMD with this speed while turning on spot.")]
            /// <summary>
            /// Lerp root towards the horizontal position of the HMD with this speed while turning on spot.
            /// </summary>
            [ShowRangeIf(0f, 50f, "mode", Mode.Animated)]
            public float rootLerpSpeedWhileTurning = 10f;

            [Tooltip("Max horizontal distance from the root to the HMD.")]
            /// <summary>
            /// Max horizontal distance from the root to the HMD.
            /// </summary>
            [ShowIf("mode", Mode.Animated)]
            public float maxRootOffset = 0.5f;

            [ShowLargeHeaderIf("Root Rotation", "mode", Mode.Animated)] [SerializeField] byte rootRotationHeader;

            [Tooltip("Max root angle from head forward while moving (ik.solver.spine.maxRootAngle).")]
            /// <summary>
            /// Max root angle from head forward while moving (ik.solver.spine.maxRootAngle).
            /// </summary>
            [ShowRangeIf(0f, 180f, "mode", Mode.Animated)]
            public float maxRootAngleMoving = 10f;

            [Tooltip("Max root angle from head forward while standing (ik.solver.spine.maxRootAngle.")]
            /// <summary>
            /// Max root angle from head forward while standing (ik.solver.spine.maxRootAngle.
            /// </summary>
            [ShowRangeIf(0f, 180f, "mode", Mode.Animated)]
            public float maxRootAngleStanding = 90f;

            /// <summary>
            /// Multiplies "VRIK_Horizontal" and "VRIK_Vertical" parameters. Larger values make steps longer and animation slower.
            /// </summary>
            [HideInInspector][SerializeField] public float stepLengthMlp = 1f;

            private Animator animator;
            private Vector3 velocityLocal, velocityLocalV;
            private Vector3 lastCorrection;
            private Vector3 lastHeadTargetPos;
            private Vector3 lastSpeedRootPos;
            private Vector3 lastEndRootPos;
            private float rootLerpSpeed, rootVelocityV;
            private float animSpeed = 1f;
            private float animSpeedV;
            private float stopMoveTimer;
            private float turn;
            private float maxRootAngleV;
            private float currentAnimationSmoothTime = 0.05f;
            private bool isMoving;
            private bool firstFrame = true;

            private static int VRIK_Horizontal;
            private static int VRIK_Vertical;
            private static int VRIK_IsMoving;
            private static int VRIK_Speed;
            private static int VRIK_Turn;
            private static bool isHashed;

            public void Initiate_Animated(Animator animator, Vector3[] positions)
            {
                this.animator = animator;

                if (animator == null && mode == Mode.Animated)
                {
                    Debug.LogError("VRIK is in Animated locomotion mode, but cannot find Animator on the VRIK root gameobject.");
                }

                ResetParams(positions);
            }

            private void ResetParams(Vector3[] positions)
            {
                lastHeadTargetPos = positions[5];
                lastSpeedRootPos = positions[0];
                lastEndRootPos = lastSpeedRootPos;
                lastCorrection = Vector3.zero;
                isMoving = false;
                currentAnimationSmoothTime = 0.05f;
                stopMoveTimer = 1f;
            }

            public void Reset_Animated(Vector3[] positions)
            {
                ResetParams(positions);

                if (animator == null) return;

                if (!isHashed)
                {
                    VRIK_Horizontal = Animator.StringToHash("VRIK_Horizontal");
                    VRIK_Vertical = Animator.StringToHash("VRIK_Vertical");
                    VRIK_IsMoving = Animator.StringToHash("VRIK_IsMoving");
                    VRIK_Speed = Animator.StringToHash("VRIK_Speed");
                    VRIK_Turn = Animator.StringToHash("VRIK_Turn");
                    isHashed = true;
                }

                if (!firstFrame)
                {
                    animator.SetFloat(VRIK_Horizontal, 0f);
                    animator.SetFloat(VRIK_Vertical, 0f);
                    animator.SetBool(VRIK_IsMoving, false);
                    animator.SetFloat(VRIK_Speed, 1f);
                    animator.SetFloat(VRIK_Turn, 0f);
                }
            }

            private void AddDeltaRotation_Animated(Quaternion delta, Vector3 pivot)
            {
                Vector3 toLastEndRootPos = lastEndRootPos - pivot;
                lastEndRootPos = pivot + delta * toLastEndRootPos;

                Vector3 toLastSpeedRootPos = lastSpeedRootPos - pivot;
                lastSpeedRootPos = pivot + delta * toLastSpeedRootPos;

                Vector3 toLastHeadTargetPos = lastHeadTargetPos - pivot;
                lastHeadTargetPos = pivot + delta * toLastHeadTargetPos;
            }

            private void AddDeltaPosition_Animated(Vector3 delta)
            {
                lastEndRootPos += delta;
                lastSpeedRootPos += delta;
                lastHeadTargetPos += delta;
            }

            private float lastVelLocalMag;

            public void Solve_Animated(IKSolverVR solver, float scale, float deltaTime)
            {
                if (animator == null)
                {
                    Debug.LogError("VRIK cannot find Animator on the VRIK root gameobject.", solver.root);
                    return;
                }

                if (deltaTime <= 0f) return;

                // Root up vector
                Vector3 rootUp = solver.rootBone.solverRotation * Vector3.up;
                
                // Substract any motion from parent transforms
                Vector3 externalDelta = solver.rootBone.solverPosition - lastEndRootPos;
                externalDelta -= animator.deltaPosition;

                // Head target position
                Vector3 headTargetPos = solver.spine.headPosition;
                Vector3 standOffsetWorld = solver.rootBone.solverRotation * new Vector3(standOffset.x, 0f, standOffset.y) * scale;
                headTargetPos += standOffsetWorld;

                if (firstFrame)
                {
                    lastHeadTargetPos = headTargetPos;

                    firstFrame = false;
                }

                // Head target velocity
                Vector3 headTargetVelocity = (headTargetPos - lastHeadTargetPos) / deltaTime;
                lastHeadTargetPos = headTargetPos;
                headTargetVelocity = V3Tools.Flatten(headTargetVelocity, rootUp);

                // Head target offset
                Vector3 offset = headTargetPos - solver.rootBone.solverPosition;
                offset -= externalDelta;
                offset -= lastCorrection;
                offset = V3Tools.Flatten(offset, rootUp);
                
                // Turning
                Vector3 headForward = (solver.spine.IKRotationHead * solver.spine.anchorRelativeToHead) * Vector3.forward;
                headForward.y = 0f;
                Vector3 headForwardLocal = Quaternion.Inverse(solver.rootBone.solverRotation) * headForward;
                float angle = Mathf.Atan2(headForwardLocal.x, headForwardLocal.z) * Mathf.Rad2Deg;
                angle += solver.spine.rootHeadingOffset;
                float turnTarget = angle / 90f;
                bool isTurning = true;
                if (Mathf.Abs(turnTarget) < 0.2f)
                {
                    turnTarget = 0f;
                    isTurning = false;
                }

                turn = Mathf.Lerp(turn, turnTarget, Time.deltaTime * 3f);
                animator.SetFloat(VRIK_Turn, turn * 2f);

                // Local Velocity, animation smoothing
                Vector3 velocityLocalTarget = Quaternion.Inverse(solver.readRotations[0]) * (headTargetVelocity + offset);
                velocityLocalTarget *= weight * stepLengthMlp;

                float animationSmoothTimeTarget = isTurning && !isMoving ? 0.2f : animationSmoothTime;
                currentAnimationSmoothTime = Mathf.Lerp(currentAnimationSmoothTime, animationSmoothTimeTarget, deltaTime * 20f);
                
                velocityLocal = Vector3.SmoothDamp(velocityLocal, velocityLocalTarget, ref velocityLocalV, currentAnimationSmoothTime, Mathf.Infinity, deltaTime);
                float velLocalMag = velocityLocal.magnitude / stepLengthMlp;

                //animator.SetBool("VRIK_StartWithRightFoot", velocityLocal.x >= 0f);
                animator.SetFloat(VRIK_Horizontal, velocityLocal.x / scale);
                animator.SetFloat(VRIK_Vertical, velocityLocal.z / scale);

                // Is Moving
                float m = moveThreshold * scale;
                if (isMoving) m *= 0.9f;
                bool isMovingRaw = velocityLocal.sqrMagnitude > m * m;
                if (isMovingRaw) stopMoveTimer = 0f;
                else stopMoveTimer += deltaTime;
                isMoving = stopMoveTimer < 0.05f;

                // Max root angle
                float maxRootAngleTarget = isMoving ? maxRootAngleMoving : maxRootAngleStanding;
                solver.spine.maxRootAngle = Mathf.SmoothDamp(solver.spine.maxRootAngle, maxRootAngleTarget, ref maxRootAngleV, 0.2f, Mathf.Infinity, deltaTime);

                animator.SetBool(VRIK_IsMoving, isMoving);

                // Animation speed
                Vector3 currentRootPos = solver.rootBone.solverPosition;
                currentRootPos -= externalDelta;
                currentRootPos -= lastCorrection;

                Vector3 rootVelocity = (currentRootPos - lastSpeedRootPos) / deltaTime;
                lastSpeedRootPos = solver.rootBone.solverPosition;
                float rootVelocityMag = rootVelocity.magnitude;

                float animSpeedTarget = minAnimationSpeed;
                if (rootVelocityMag > 0f && isMovingRaw)
                {
                    animSpeedTarget = animSpeed * (velLocalMag / rootVelocityMag);
                }
                animSpeedTarget = Mathf.Clamp(animSpeedTarget, minAnimationSpeed, maxAnimationSpeed);
                animSpeed = Mathf.SmoothDamp(animSpeed, animSpeedTarget, ref animSpeedV, 0.05f, Mathf.Infinity, deltaTime);
                animSpeed = Mathf.Lerp(1f, animSpeed, weight);

                animator.SetFloat(VRIK_Speed, animSpeed);

                // Is Stopping
                AnimatorTransitionInfo transInfo = animator.GetAnimatorTransitionInfo(0);
                bool isStopping = transInfo.IsUserName("VRIK_Stop");
                
                // Root lerp speed
                float rootLerpSpeedTarget = 0;
                if (isMoving) rootLerpSpeedTarget = rootLerpSpeedWhileMoving;
                if (isStopping) rootLerpSpeedTarget = rootLerpSpeedWhileStopping;
                if (isTurning) rootLerpSpeedTarget = rootLerpSpeedWhileTurning;

                rootLerpSpeedTarget *= Mathf.Max(headTargetVelocity.magnitude, 0.2f);
                rootLerpSpeed = Mathf.Lerp(rootLerpSpeed, rootLerpSpeedTarget, deltaTime * 20f);

                // Root lerp and limits
                headTargetPos += V3Tools.ExtractVertical(solver.rootBone.solverPosition - headTargetPos, rootUp, 1f);

                if (maxRootOffset > 0f)
                {
                    // Lerp towards head target position
                    Vector3 p = solver.rootBone.solverPosition;
                    
                    if (rootLerpSpeed > 0f)
                    {
                        solver.rootBone.solverPosition = Vector3.Lerp(solver.rootBone.solverPosition, headTargetPos, rootLerpSpeed * deltaTime * weight);
                    }

                    lastCorrection = solver.rootBone.solverPosition - p;

                    // Max offset
                    offset = headTargetPos - solver.rootBone.solverPosition;
                    offset = V3Tools.Flatten(offset, rootUp);
                    float offsetMag = offset.magnitude;

                    if (offsetMag > maxRootOffset)
                    {
                        lastCorrection += (offset - (offset / offsetMag) * maxRootOffset) * weight;
                        solver.rootBone.solverPosition += lastCorrection;
                    }
                } else
                {
                    // Snap to head target position
                    lastCorrection = (headTargetPos - solver.rootBone.solverPosition) * weight;
                    solver.rootBone.solverPosition += lastCorrection;
                }

                lastEndRootPos = solver.rootBone.solverPosition;
            }
        }
    }
}