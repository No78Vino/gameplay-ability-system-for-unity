using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK {

	/// <summary>
	/// The base abstract class for all class that are translating a hierarchy of bones to match the translation of bones in another hierarchy.
	/// </summary>
	public abstract class Poser: SolverManager {

		/// <summary>
		/// Reference to the other Transform (should be identical to this one)
		/// </summary>
		public Transform poseRoot;
		/// <summary>
		/// The master weight.
		/// </summary>
		[Range(0f, 1f)] public float weight = 1f;
		/// <summary>
		/// Weight of localRotation matching
		/// </summary>
		[Range(0f, 1f)] public float localRotationWeight = 1f;
		/// <summary>
		/// Weight of localPosition matching
		/// </summary>
		[Range(0f, 1f)] public float localPositionWeight;

		/// <summary>
		/// Map this instance to the poseRoot.
		/// </summary>
		public abstract void AutoMapping();
		public virtual void AutoMapping(Transform[] bones) { }

		/// <summary>
		/// For manual update of the poser.
		/// </summary>
		public void UpdateManual(float deltaTime) {

			UpdatePoser();
			UpdateLerping(deltaTime);
			if (storeCurrentPoseFlag) OnStoreCurrentPoseFlag();
		}

		public void SetPoseRoot(Transform newPoseRoot, Transform[] autoMappingBones, float blendSpeed)
        {
			this.blendSpeed = blendSpeed;

			if (newPoseRoot != null && poseRoot != null && weight > 0f)
			{
				nextPoseRoot = newPoseRoot;
				this.autoMappingBones = autoMappingBones;
				storeCurrentPoseFlag = true;
			} else
            {
				poseRoot = newPoseRoot;
				AutoMapping(autoMappingBones);
				storeCurrentPoseFlag = false;
            }
        }

		private bool initiated;
		protected abstract void InitiatePoser();
		protected abstract void UpdatePoser();
		protected abstract void StoreCurrentPose();
		protected abstract void BlendFromLastPose(float lerpTimer);
		protected abstract void FixPoserTransforms();

		protected Transform nextPoseRoot;
		protected bool storeCurrentPoseFlag;
		private bool isLerping;
		private float lerpTimer;
		private Transform[] autoMappingBones = new Transform[0];
		private float blendSpeed;

		private void UpdateLerping(float deltaTime)
        {
			if (!isLerping) return;
			
			lerpTimer = Mathf.MoveTowards(lerpTimer, 1f, deltaTime * blendSpeed);
			BlendFromLastPose(lerpTimer);

			if (lerpTimer >= 1f)
            {
				isLerping = false;
				lerpTimer = 0f;
            }
        }

		/*
		 * Updates the solver. If you need full control of the execution order of your IK solvers, disable this script and call UpdateSolver() instead.
		 * */
		protected override void UpdateSolver() {
			if (!initiated) InitiateSolver();
			if (!initiated) return;

			UpdatePoser();
			UpdateLerping(Time.deltaTime);
			if (storeCurrentPoseFlag) OnStoreCurrentPoseFlag();
		}

		private void OnStoreCurrentPoseFlag()
        {
			StoreCurrentPose();
			isLerping = true;
			lerpTimer = 0f;
			poseRoot = nextPoseRoot;
			AutoMapping(autoMappingBones);
			nextPoseRoot = null;

			storeCurrentPoseFlag = false;
		}

		/*
		 * Initiates the %IK solver
		 * */
		protected override void InitiateSolver() {
			if (initiated) return;
			InitiatePoser();
			initiated = true;
		}
		
		protected override void FixTransforms() {
			if (!initiated) return;
 			FixPoserTransforms();
		}
	}
}
