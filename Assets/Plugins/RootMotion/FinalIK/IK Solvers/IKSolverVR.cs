using UnityEngine;
using System.Collections;
using System;
using RootMotion;

namespace RootMotion.FinalIK {

	/// <summary>
	/// Hybrid %IK solver designed for mapping a character to a VR headset and 2 hand controllers 
	/// </summary>
	[System.Serializable]
	public partial class IKSolverVR: IKSolver {

        #region Wrapper

        public Animator animator { get; private set; }

		/// <summary>
		/// Sets this VRIK up to the specified bone references.
		/// </summary>
		public void SetToReferences(VRIK.References references) {
			if (!references.isFilled) {
				Debug.LogError("Invalid references, one or more Transforms are missing.");
				return;
			}

            animator = references.root.GetComponent<Animator>();

			solverTransforms = references.GetTransforms();

			hasChest = solverTransforms [3] != null;
			hasNeck = solverTransforms[4] != null;
			hasShoulders = solverTransforms[6] != null && solverTransforms[10] != null;
			hasToes = solverTransforms[17] != null && solverTransforms[21] != null;
            hasLegs = solverTransforms[14] != null;
            hasArms = solverTransforms[7] != null;

			readPositions = new Vector3[solverTransforms.Length];
			readRotations = new Quaternion[solverTransforms.Length];
			
			DefaultAnimationCurves();
			GuessHandOrientations(references, true);
		}

		/// <summary>
		/// Guesses the hand bones orientations ('Wrist To Palm Axis' and "Palm To Thumb Axis" of the arms) based on the provided references. if onlyIfZero is true, will only guess an orientation axis if it is Vector3.zero.
		/// </summary>
		public void GuessHandOrientations(VRIK.References references, bool onlyIfZero) {
			if (!references.isFilled) {
				Debug.LogError("VRIK References are not filled in, can not guess hand orientations. Right-click on VRIK header and slect 'Guess Hand Orientations' when you have filled in the References.", references.root);
				return;
			}
			
			if (leftArm.wristToPalmAxis == Vector3.zero || !onlyIfZero) {
				leftArm.wristToPalmAxis = VRIKCalibrator.GuessWristToPalmAxis(references.leftHand, references.leftForearm);
			}
			
			if (leftArm.palmToThumbAxis == Vector3.zero || !onlyIfZero) {
				leftArm.palmToThumbAxis = VRIKCalibrator.GuessPalmToThumbAxis(references.leftHand, references.leftForearm);
			}
			
			if (rightArm.wristToPalmAxis == Vector3.zero || !onlyIfZero) {
				rightArm.wristToPalmAxis = VRIKCalibrator.GuessWristToPalmAxis(references.rightHand, references.rightForearm);
			}
			
			if (rightArm.palmToThumbAxis == Vector3.zero || !onlyIfZero) {
				rightArm.palmToThumbAxis = VRIKCalibrator.GuessPalmToThumbAxis(references.rightHand, references.rightForearm);
			}
		}

		/// <summary>
		/// Set default values for the animation curves if they have no keys.
		/// </summary>
		public void DefaultAnimationCurves() {
			if (locomotion.stepHeight == null) locomotion.stepHeight = new AnimationCurve();
			if (locomotion.heelHeight == null) locomotion.heelHeight = new AnimationCurve ();
			
			if (locomotion.stepHeight.keys.Length == 0) {
				locomotion.stepHeight.keys = GetSineKeyframes(0.03f);
			}
			
			if (locomotion.heelHeight.keys.Length == 0) {
				locomotion.heelHeight.keys = GetSineKeyframes(0.03f);
			}
		}

		/// <summary>
		/// Adds position offset to a body part. Position offsets add to the targets in VRIK.
		/// </summary>
		public void AddPositionOffset(PositionOffset positionOffset, Vector3 value) {
			switch(positionOffset) {
			case PositionOffset.Pelvis: spine.pelvisPositionOffset += value; return;
			case PositionOffset.Chest: spine.chestPositionOffset += value; return;
			case PositionOffset.Head: spine.headPositionOffset += value; return;
			case PositionOffset.LeftHand: leftArm.handPositionOffset += value; return;
			case PositionOffset.RightHand: rightArm.handPositionOffset += value; return;
			case PositionOffset.LeftFoot: leftLeg.footPositionOffset += value; return;
			case PositionOffset.RightFoot: rightLeg.footPositionOffset += value; return;
			case PositionOffset.LeftHeel: leftLeg.heelPositionOffset += value; return;
			case PositionOffset.RightHeel: rightLeg.heelPositionOffset += value; return;
			}
		}

		/// <summary>
		/// Adds rotation offset to a body part. Rotation offsets add to the targets in VRIK
		/// </summary>
		public void AddRotationOffset(RotationOffset rotationOffset, Vector3 value) {
			AddRotationOffset(rotationOffset, Quaternion.Euler(value));
		}

		/// <summary>
		/// Adds rotation offset to a body part. Rotation offsets add to the targets in VRIK
		/// </summary>
		public void AddRotationOffset(RotationOffset rotationOffset, Quaternion value) {
			switch(rotationOffset) {
			case RotationOffset.Pelvis: spine.pelvisRotationOffset = value * spine.pelvisRotationOffset; return;
			case RotationOffset.Chest: spine.chestRotationOffset = value * spine.chestRotationOffset; return;
			case RotationOffset.Head: spine.headRotationOffset = value * spine.headRotationOffset; return;
			}
		}

		/// <summary>
		/// Call this in each Update if your avatar is standing on a moving platform
		/// </summary>
		public void AddPlatformMotion(Vector3 deltaPosition, Quaternion deltaRotation, Vector3 platformPivot) {
			locomotion.AddDeltaPosition (deltaPosition);
			raycastOriginPelvis += deltaPosition;

			locomotion.AddDeltaRotation (deltaRotation, platformPivot);
			spine.faceDirection = deltaRotation * spine.faceDirection;
		}

		/// <summary>
		/// Resets all tweens, blendings and lerps. Call this after you have teleported the character.
		/// </summary>
		public void Reset() {
			if (!initiated) return;

			UpdateSolverTransforms();
			Read(readPositions, readRotations, hasChest, hasNeck, hasShoulders, hasToes, hasLegs, hasArms);
			
			spine.faceDirection = rootBone.readRotation * Vector3.forward;

            if (hasLegs)
            {
                locomotion.Reset(readPositions, readRotations);
                raycastOriginPelvis = spine.pelvis.readPosition;
            }
		}

		public override void StoreDefaultLocalState() {
			for (int i = 1; i < solverTransforms.Length; i++) {
				if (solverTransforms[i] != null) {
					defaultLocalPositions[i - 1] = solverTransforms[i].localPosition;
					defaultLocalRotations[i - 1] = solverTransforms[i].localRotation;
				}
			}
		}
		
		public override void FixTransforms() {
			if (!initiated) return;
            if (LOD >= 2) return;

			for (int i = 1; i < solverTransforms.Length; i++) {
				if (solverTransforms[i] != null) {
					bool isPelvis = i == 1;
					
                    bool isArmStretchable = i == 8 || i == 9 || i == 12 || i == 13;
                    bool isLegStretchable = (i >= 15 && i <= 17) || (i >= 19 && i <= 21);

                    if (isPelvis || isArmStretchable || isLegStretchable) {
						solverTransforms[i].localPosition = defaultLocalPositions[i - 1];
					}
					solverTransforms[i].localRotation = defaultLocalRotations[i - 1];
				}
			}
		}
		
		public override IKSolver.Point[] GetPoints() {
			Debug.LogError("GetPoints() is not applicable to IKSolverVR.");
			return null;
		}
		
		public override IKSolver.Point GetPoint(Transform transform) {
			Debug.LogError("GetPoint is not applicable to IKSolverVR.");
			return null;
		}
		
		public override bool IsValid(ref string message) {
			if (solverTransforms == null || solverTransforms.Length == 0) {
				message = "Trying to initiate IKSolverVR with invalid bone references.";
				return false;
			}
			
			if (leftArm.wristToPalmAxis == Vector3.zero) {
				message = "Left arm 'Wrist To Palm Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the wrist towards the palm. If the arrow points away from the palm, axis must be negative.";
				return false;
			}
			
			if (rightArm.wristToPalmAxis == Vector3.zero) {
				message = "Right arm 'Wrist To Palm Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the wrist towards the palm. If the arrow points away from the palm, axis must be negative.";
				return false;
			}
			
			if (leftArm.palmToThumbAxis == Vector3.zero) {
				message = "Left arm 'Palm To Thumb Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the palm towards the thumb. If the arrow points away from the thumb, axis must be negative.";
				return false;
			}
			
			if (rightArm.palmToThumbAxis == Vector3.zero) {
				message = "Right arm 'Palm To Thumb Axis' needs to be set in VRIK. Please select the hand bone, set it to the axis that points from the palm towards the thumb. If the arrow points away from the thumb, axis must be negative.";
				return false;
			}
			
			return true;
		}

		private Transform[] solverTransforms = new Transform[0];
        private bool hasChest, hasNeck, hasShoulders, hasToes, hasLegs, hasArms;
		private Vector3[] readPositions = new Vector3[0];
		private Quaternion[] readRotations = new Quaternion[0];
		private Vector3[] solvedPositions = new Vector3[22];
		private Quaternion[] solvedRotations = new Quaternion[22];
		//private Vector3 defaultPelvisLocalPosition;
		private Quaternion[] defaultLocalRotations = new Quaternion[21];
		private Vector3[] defaultLocalPositions = new Vector3[21];
		
		private Vector3 GetNormal(Transform[] transforms) {
			Vector3 normal = Vector3.zero;

			Vector3 centroid = Vector3.zero;
			for (int i = 0; i < transforms.Length; i++) {
				centroid += transforms[i].position;
			}
			centroid /= transforms.Length;

			for (int i = 0; i < transforms.Length - 1; i++) {
				normal += Vector3.Cross(transforms[i].position - centroid, transforms[i + 1].position - centroid).normalized;
			}
			
			return normal;
		}
        
		private static Keyframe[] GetSineKeyframes(float mag) {
			Keyframe[] keys = new Keyframe[3];
			keys[0].time = 0f;
			keys[0].value = 0f;
			keys[1].time = 0.5f;
			keys[1].value = mag;
			keys[2].time = 1f;
			keys[2].value = 0f;
			return keys;
		}

		private void UpdateSolverTransforms() {
			for (int i = 0; i < solverTransforms.Length; i++) {
				if (solverTransforms[i] != null) {
					readPositions[i] = solverTransforms[i].position;
					readRotations[i] = solverTransforms[i].rotation;
				}
			}
		}

		protected override void OnInitiate() {
            UpdateSolverTransforms();
			Read(readPositions, readRotations, hasChest, hasNeck, hasShoulders, hasToes, hasLegs, hasArms);
		}

		protected override void OnUpdate() {
			if (IKPositionWeight > 0f) {
                if (LOD < 2)
                {
                    bool read = false;

                    if (lastLOD != LOD)
                    {
                        if (lastLOD == 2)
                        {
                            spine.faceDirection = rootBone.readRotation * Vector3.forward;

                            if (hasLegs)
                            {
                                // Teleport to the current position/rotation if resuming from culled LOD with locomotion enabled
                                if (locomotion.weight > 0f)
                                {
                                    root.position = new Vector3(spine.headTarget.position.x, root.position.y, spine.headTarget.position.z);
                                    Vector3 forward = spine.faceDirection;
                                    forward.y = 0f;
                                    root.rotation = Quaternion.LookRotation(forward, root.up);

                                    UpdateSolverTransforms();
                                    Read(readPositions, readRotations, hasChest, hasNeck, hasShoulders, hasToes, hasLegs, hasArms);
                                    read = true;

                                    locomotion.Reset(readPositions, readRotations);
                                }

                                raycastOriginPelvis = spine.pelvis.readPosition;
                            }
                        }
                    }

                    if (!read)
                    {
                        UpdateSolverTransforms();
                        Read(readPositions, readRotations, hasChest, hasNeck, hasShoulders, hasToes, hasLegs, hasArms);
                    }

                    Solve();
                    Write();

                    WriteTransforms();
                }
                else
                {
                    // Culled
                    if (locomotion.weight > 0f)
                    {
                        root.position = new Vector3(spine.headTarget.position.x, root.position.y, spine.headTarget.position.z);
                        Vector3 forward = spine.headTarget.rotation * spine.anchorRelativeToHead * Vector3.forward;
                        forward.y = 0f;
                        root.rotation = Quaternion.LookRotation(forward, root.up);
                    }
                }
			}

            lastLOD = LOD;
        }

		private void WriteTransforms() {
			for (int i = 0; i < solverTransforms.Length; i++) {
				if (solverTransforms[i] != null) {
					bool isRootOrPelvis = i < 2;
                    bool isArmStretchable = i == 8 || i == 9 || i == 12 || i == 13;
                    bool isLegStretchable = (i >= 15 && i <= 17) || (i >= 19 && i <= 21);

                    if (LOD > 0)
                    {
                        isArmStretchable = false;
                        isLegStretchable = false;
                    }

                    if (isRootOrPelvis) {
						solverTransforms[i].position = V3Tools.Lerp(solverTransforms[i].position, GetPosition(i), IKPositionWeight);
					}

					if (isArmStretchable || isLegStretchable) {
                        if (IKPositionWeight < 1f) { 
                            Vector3 localPosition = solverTransforms[i].localPosition;
                            solverTransforms[i].position = V3Tools.Lerp(solverTransforms[i].position, GetPosition(i), IKPositionWeight);
                            solverTransforms[i].localPosition = Vector3.Project(solverTransforms[i].localPosition, localPosition);
                        } else
                        {
                            solverTransforms[i].position = V3Tools.Lerp(solverTransforms[i].position, GetPosition(i), IKPositionWeight);
                        }
                    }

					solverTransforms[i].rotation = QuaTools.Lerp(solverTransforms[i].rotation, GetRotation(i), IKPositionWeight);
                }
			}
		}

		#endregion Wrapper

		#region Generic API

		private Vector3 rootV;
		private Vector3 rootVelocity;
		private Vector3 bodyOffset;
		private int supportLegIndex;
        private int lastLOD;

        private void Read(Vector3[] positions, Quaternion[] rotations, bool hasChest, bool hasNeck, bool hasShoulders, bool hasToes, bool hasLegs, bool hasArms) {
			if (rootBone == null) {
				rootBone = new VirtualBone (positions [0], rotations [0]);
			} else {
				rootBone.Read (positions [0], rotations [0]);
			}

			spine.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, hasLegs, 0, 1);

            if (hasArms)
            {
                leftArm.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, hasLegs, hasChest ? 3 : 2, 6);
                rightArm.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, hasLegs, hasChest ? 3 : 2, 10);
            }

            if (hasLegs) {
                leftLeg.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, hasLegs, 1, 14);
                rightLeg.Read(positions, rotations, hasChest, hasNeck, hasShoulders, hasToes, hasLegs, 1, 18);
            }

			for (int i = 0; i < rotations.Length; i++) {
				this.solvedPositions[i] = positions[i];
				this.solvedRotations[i] = rotations[i];
			}

			if (!initiated) {
				if (hasLegs) legs = new Leg[2] { leftLeg, rightLeg };
				if (hasArms) arms = new Arm[2] { leftArm, rightArm };

				if (hasLegs) locomotion.Initiate(animator, positions, rotations, hasToes, scale);
				raycastOriginPelvis = spine.pelvis.readPosition;
				spine.faceDirection = readRotations[0] * Vector3.forward;
			}
		}

        private void Solve() {
            if (scale <= 0f)
            {
                Debug.LogError("VRIK solver scale <= 0, can not solve!");
                return;
            }

            if (hasLegs && lastLocomotionWeight <= 0f && locomotion.weight > 0f) locomotion.Reset(readPositions, readRotations);

            spine.SetLOD(LOD);
            if (hasArms) foreach (Arm arm in arms) arm.SetLOD(LOD);
            if (hasLegs) foreach (Leg leg in legs) leg.SetLOD(LOD);

            // Pre-Solving
            spine.PreSolve(scale);
			if (hasArms) foreach (Arm arm in arms) arm.PreSolve(scale);
			if (hasLegs) foreach (Leg leg in legs) leg.PreSolve(scale);

			// Applying spine and arm offsets
			if (hasArms) foreach (Arm arm in arms) arm.ApplyOffsets(scale);
			spine.ApplyOffsets(scale);

			// Spine
			spine.Solve(animator, rootBone, legs, arms, scale);

			if (hasLegs && spine.pelvisPositionWeight > 0f && plantFeet) {
				Warning.Log("If VRIK 'Pelvis Position Weight' is > 0, 'Plant Feet' should be disabled to improve performance and stability.", root);
			}

            float deltaTime = Time.deltaTime;

            // Locomotion
            if (hasLegs) {
                if (locomotion.weight > 0f)
                {
                    switch (locomotion.mode)
                    {
                        case Locomotion.Mode.Procedural:
                            Vector3 leftFootPosition = Vector3.zero;
                            Vector3 rightFootPosition = Vector3.zero;
                            Quaternion leftFootRotation = Quaternion.identity;
                            Quaternion rightFootRotation = Quaternion.identity;
                            float leftFootOffset = 0f;
                            float rightFootOffset = 0f;
                            float leftHeelOffset = 0f;
                            float rightHeelOffset = 0f;

                            locomotion.Solve_Procedural(rootBone, spine, leftLeg, rightLeg, leftArm, rightArm, supportLegIndex, out leftFootPosition, out rightFootPosition, out leftFootRotation, out rightFootRotation, out leftFootOffset, out rightFootOffset, out leftHeelOffset, out rightHeelOffset, scale, deltaTime);

                            leftFootPosition += root.up * leftFootOffset;
                            rightFootPosition += root.up * rightFootOffset;

                            leftLeg.footPositionOffset += (leftFootPosition - leftLeg.lastBone.solverPosition) * IKPositionWeight * (1f - leftLeg.positionWeight) * locomotion.weight;
                            rightLeg.footPositionOffset += (rightFootPosition - rightLeg.lastBone.solverPosition) * IKPositionWeight * (1f - rightLeg.positionWeight) * locomotion.weight;

                            leftLeg.heelPositionOffset += root.up * leftHeelOffset * locomotion.weight;
                            rightLeg.heelPositionOffset += root.up * rightHeelOffset * locomotion.weight;

                            Quaternion rotationOffsetLeft = QuaTools.FromToRotation(leftLeg.lastBone.solverRotation, leftFootRotation);
                            Quaternion rotationOffsetRight = QuaTools.FromToRotation(rightLeg.lastBone.solverRotation, rightFootRotation);

                            rotationOffsetLeft = Quaternion.Lerp(Quaternion.identity, rotationOffsetLeft, IKPositionWeight * (1f - leftLeg.rotationWeight) * locomotion.weight);
                            rotationOffsetRight = Quaternion.Lerp(Quaternion.identity, rotationOffsetRight, IKPositionWeight * (1f - rightLeg.rotationWeight) * locomotion.weight);

                            leftLeg.footRotationOffset = rotationOffsetLeft * leftLeg.footRotationOffset;
                            rightLeg.footRotationOffset = rotationOffsetRight * rightLeg.footRotationOffset;

                            Vector3 footPositionC = Vector3.Lerp(leftLeg.position + leftLeg.footPositionOffset, rightLeg.position + rightLeg.footPositionOffset, 0.5f);
                            footPositionC = V3Tools.PointToPlane(footPositionC, rootBone.solverPosition, root.up);

                            Vector3 p = rootBone.solverPosition + rootVelocity * deltaTime * 2f * locomotion.weight;
                            p = Vector3.Lerp(p, footPositionC, deltaTime * locomotion.rootSpeed * locomotion.weight);
                            rootBone.solverPosition = p;

                            rootVelocity += (footPositionC - rootBone.solverPosition) * deltaTime * 10f;
                            Vector3 rootVelocityV = V3Tools.ExtractVertical(rootVelocity, root.up, 1f);
                            rootVelocity -= rootVelocityV;

                            float bodyYOffset = Mathf.Min(leftFootOffset + rightFootOffset, locomotion.maxBodyYOffset * scale);
                            bodyOffset = Vector3.Lerp(bodyOffset, root.up * bodyYOffset, deltaTime * 3f);
                            bodyOffset = Vector3.Lerp(Vector3.zero, bodyOffset, locomotion.weight);

                            break;
                        case Locomotion.Mode.Animated:
                            if (lastLocomotionWeight <= 0f) locomotion.Reset_Animated(readPositions);
                            locomotion.Solve_Animated(this, scale, deltaTime);
                            break;
                    }
                } else
                {
                    if (lastLocomotionWeight > 0f) locomotion.Reset_Animated(readPositions);
                }
			}

            lastLocomotionWeight = locomotion.weight;

            // Legs
            if (hasLegs)
            {
                foreach (Leg leg in legs)
                {
                    leg.ApplyOffsets(scale);
                }
                if (!plantFeet || LOD > 0)
                {
                    spine.InverseTranslateToHead(legs, false, false, bodyOffset, 1f);

                    foreach (Leg leg in legs) leg.TranslateRoot(spine.pelvis.solverPosition, spine.pelvis.solverRotation);
                    foreach (Leg leg in legs)
                    {
                        leg.Solve(true);
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        spine.InverseTranslateToHead(legs, true, true, bodyOffset, 1f);

                        foreach (Leg leg in legs) leg.TranslateRoot(spine.pelvis.solverPosition, spine.pelvis.solverRotation);
                        foreach (Leg leg in legs)
                        {
                            leg.Solve(i == 0);
                        }
                    }
                }
            } else
            {
                spine.InverseTranslateToHead(legs, false, false, bodyOffset, 1f);
            }

            // Arms
            if (hasArms)
            {
                for (int i = 0; i < arms.Length; i++)
                {
                    arms[i].TranslateRoot(spine.chest.solverPosition, spine.chest.solverRotation);
                }

                for (int i = 0; i < arms.Length; i++)
                {
                    arms[i].Solve(i == 0);
                }
            }

			// Reset offsets
			spine.ResetOffsets();
			if (hasLegs) foreach (Leg leg in legs) leg.ResetOffsets();
			if (hasArms) foreach (Arm arm in arms) arm.ResetOffsets();

            if (hasLegs)
            {
                spine.pelvisPositionOffset += GetPelvisOffset(deltaTime);
                spine.chestPositionOffset += spine.pelvisPositionOffset;
                //spine.headPositionOffset += spine.pelvisPositionOffset;
            }

			Write();

            // Find the support leg
            if (hasLegs)
            {
                supportLegIndex = -1;
                float shortestMag = Mathf.Infinity;
                for (int i = 0; i < legs.Length; i++)
                {
                    float mag = Vector3.SqrMagnitude(legs[i].lastBone.solverPosition - legs[i].bones[0].solverPosition);
                    if (mag < shortestMag)
                    {
                        supportLegIndex = i;
                        shortestMag = mag;
                    }
                }
            }
		}

        private float lastLocomotionWeight;

		private Vector3 GetPosition(int index) {
			return solvedPositions[index];
		}

		private Quaternion GetRotation(int index) {
			return solvedRotations[index];
		}

        #endregion Generic API

        [Tooltip("LOD 0: Full quality solving. LOD 1: Shoulder solving, stretching plant feet disabled, spine solving quality reduced. This provides about 30% of performance gain. LOD 2: Culled, but updating root position and rotation if locomotion is enabled.")]
        /// <summary>
        /// LOD 0: Full quality solving. LOD 1: Shoulder solving, stretching plant feet disabled, spine solving quality reduced. This provides about 30% of performance gain. LOD 2: Culled, but updating root position and rotation if locomotion is enabled.
        /// </summary>
        [Range(0, 2)] public int LOD = 0;

        [Tooltip("Scale of the character. Value of 1 means normal adult human size.")]
        /// <summary>
        /// Scale of the character. Value of 1 means normal adult human size.
        /// </summary>
        public float scale = 1f;

		[Tooltip("If true, will keep the toes planted even if head target is out of reach, so this can cause the camera to exit the head if it is too high for the model to reach. Enabling this increases the cost of the solver as the legs will have to be solved multiple times.")]
        /// <summary>
        /// If true, will keep the toes planted even if head target is out of reach, so this can cause the camera to exit the head if it is too high for the model to reach. Enabling this increases the cost of the solver as the legs will have to be solved multiple times.
        /// </summary>
        public bool plantFeet = true;

		/// <summary>
		/// Gets the root bone.
		/// </summary>
		[HideInInspector] public VirtualBone rootBone { get; private set; }

		[Tooltip("The spine solver.")]
		/// <summary>
		/// The spine solver.
		/// </summary>
		public Spine spine = new Spine();

		[Tooltip("The left arm solver.")]
		/// <summary>
		/// The left arm solver.
		/// </summary>
		public Arm leftArm = new Arm();

		[Tooltip("The right arm solver.")]
		/// <summary>
		/// The right arm solver.
		/// </summary>
		public Arm rightArm = new Arm();

		[Tooltip("The left leg solver.")]
		/// <summary>
		/// The left leg solver.
		/// </summary>
		public Leg leftLeg = new Leg();

		[Tooltip("The right leg solver.")]
		/// <summary>
		/// The right leg solver.
		/// </summary>
		public Leg rightLeg = new Leg();

        [Tooltip("Procedural leg shuffling for stationary VR games. Not designed for roomscale and thumbstick locomotion. For those it would be better to use a strafing locomotion blend tree to make the character follow the horizontal direction towards the HMD by root motion or script.")]
        /// <summary>
        /// Procedural leg shuffling for stationary VR games. Not designed for roomscale and thumbstick locomotion. For those it would be better to use a strafing locomotion blend tree to make the character follow the horizontal direction towards the HMD by root motion or script.
        /// </summary>
        public Locomotion locomotion = new Locomotion();

		private Leg[] legs = new Leg[2];
		private Arm[] arms = new Arm[2];
		private Vector3 headPosition;
		private Vector3 headDeltaPosition;
		private Vector3 raycastOriginPelvis;
		private Vector3 lastOffset;
		private Vector3 debugPos1;
		private Vector3 debugPos2;
		private Vector3 debugPos3;
		private Vector3 debugPos4;

		private void Write() {
			solvedPositions[0] = rootBone.solverPosition;
			solvedRotations[0] = rootBone.solverRotation;
			spine.Write(ref solvedPositions, ref solvedRotations);

            if (hasLegs)
            {
                foreach (Leg leg in legs) leg.Write(ref solvedPositions, ref solvedRotations);
            }
            if (hasArms)
            {
                foreach (Arm arm in arms) arm.Write(ref solvedPositions, ref solvedRotations);
            }
		}

		private Vector3 GetPelvisOffset(float deltaTime) {
			if (locomotion.weight <= 0f) return Vector3.zero;
			if (locomotion.blockingLayers == -1) return Vector3.zero;

			// Origin to pelvis transform position
			Vector3 sampledOrigin = raycastOriginPelvis;
			sampledOrigin.y = spine.pelvis.solverPosition.y;
			Vector3 origin = spine.pelvis.readPosition;
			origin.y = spine.pelvis.solverPosition.y;
			Vector3 direction = origin - sampledOrigin;
			RaycastHit hit;

			//debugPos4 = sampledOrigin;

			if (locomotion.raycastRadius <= 0f) {
				if (Physics.Raycast(sampledOrigin, direction, out hit, direction.magnitude * 1.1f, locomotion.blockingLayers)) {
					origin = hit.point;
				}
			} else {
				if (Physics.SphereCast(sampledOrigin, locomotion.raycastRadius * 1.1f, direction, out hit, direction.magnitude, locomotion.blockingLayers)) {
					origin = sampledOrigin + direction.normalized * hit.distance / 1.1f;
				}
			}

			Vector3 position = spine.pelvis.solverPosition;
			direction = position - origin;

			//debugPos1 = origin;
			//debugPos2 = position;

			if (locomotion.raycastRadius <= 0f) {
				if (Physics.Raycast(origin, direction, out hit, direction.magnitude, locomotion.blockingLayers)) {
					position = hit.point;
				}

			} else {
				if (Physics.SphereCast(origin, locomotion.raycastRadius, direction, out hit, direction.magnitude, locomotion.blockingLayers)) {
					position = origin + direction.normalized * hit.distance;
				}
			}

			lastOffset = Vector3.Lerp(lastOffset, Vector3.zero, deltaTime * 3f);
			position += Vector3.ClampMagnitude(lastOffset, 0.75f);
			position.y = spine.pelvis.solverPosition.y;

			//debugPos3 = position;

			lastOffset = Vector3.Lerp(lastOffset, position - spine.pelvis.solverPosition, deltaTime * 15f);
			return lastOffset;
		}
	}
}