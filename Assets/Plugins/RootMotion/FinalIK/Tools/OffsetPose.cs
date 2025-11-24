using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK {

	/// <summary>
	/// Definition of FBBIK Offset pose.
	/// </summary>
	public class OffsetPose: MonoBehaviour {

		/// <summary>
		/// State of an effector in this pose
		/// </summary>
		[System.Serializable]
		public class EffectorLink {

			[Tooltip("The effector type (this is just an enum)")] public FullBodyBipedEffector effector;
			[Tooltip("Offset of the effector in this pose")] public Vector3 offset;
			[Tooltip("Pin position relative to the solver root Transform")] public Vector3 pin;
			[Tooltip("Pin weight vector")] public Vector3 pinWeight;
			[Tooltip("Only applies for end effectors (hands, feet)")] public Vector3 rotationOffset;

			// Apply positionOffset to the effector
			public void Apply(IKSolverFullBodyBiped solver, float weight, Quaternion rotation) {
				var e = solver.GetEffector(effector);

				// Offset
				e.positionOffset += rotation * offset * weight;
				
				// Calculating pinned position
				Vector3 pinPosition = solver.GetRoot().position + rotation * pin;
				Vector3 pinPositionOffset = pinPosition - e.bone.position;
				
				Vector3 pinWeightVector = pinWeight * Mathf.Abs(weight);
				
				// Lerping to pinned position
				e.positionOffset = new Vector3(
					Mathf.Lerp(e.positionOffset.x, pinPositionOffset.x, pinWeightVector.x),
					Mathf.Lerp(e.positionOffset.y, pinPositionOffset.y, pinWeightVector.y),
					Mathf.Lerp(e.positionOffset.z, pinPositionOffset.z, pinWeightVector.z)
					);

				if (e.isEndEffector) e.bone.localRotation *= Quaternion.Euler(rotationOffset * weight);
			}
		}

		public EffectorLink[] effectorLinks = new EffectorLink[0];

		// Apply positionOffsets of all the EffectorLinks
		public void Apply(IKSolverFullBodyBiped solver, float weight) {
			for (int i = 0; i < effectorLinks.Length; i++) effectorLinks[i].Apply(solver, weight, solver.GetRoot().rotation);
		}

		public void Apply(IKSolverFullBodyBiped solver, float weight, Quaternion rotation) {
			for (int i = 0; i < effectorLinks.Length; i++) effectorLinks[i].Apply(solver, weight, rotation);
		}
	}
}
