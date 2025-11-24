using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK {

	/// <summary>
	/// Hybrid %IK solver designed for mapping a character to a VR headset and 2 hand controllers 
	/// </summary>
	public partial class IKSolverVR: IKSolver {

		[System.Serializable]
		public partial class Locomotion {

            [System.Serializable]
            public enum Mode
            {
                Procedural = 0,
                Animated = 1,
            }

            [Tooltip("Procedural (legacy) or animated locomotion.")]
            /// <summary>
            /// Procedural (legacy) or animated locomotion.
            /// </summary>
            public Mode mode;

            [Tooltip("Used for blending in/out of procedural/animated locomotion.")]
            /// <summary>
            /// Used for blending in/out of procedural/animated locomotion.
            /// </summary>
            [Range(0f, 1f)]
            public float weight = 1f;

            public void Initiate(Animator animator, Vector3[] positions, Quaternion[] rotations, bool hasToes, float scale) {
				
                Initiate_Procedural(positions, rotations, hasToes, scale);
                Initiate_Animated(animator, positions);
			}

			public void Reset(Vector3[] positions, Quaternion[] rotations) {
                Reset_Procedural(positions, rotations);
                Reset_Animated(positions);
            }

            public void Relax()
            {
                Relax_Procedural();
            }

			public void AddDeltaRotation(Quaternion delta, Vector3 pivot) {
                AddDeltaRotation_Procedural(delta, pivot);
                AddDeltaRotation_Animated(delta, pivot);
            }
			
			public void AddDeltaPosition(Vector3 delta) {
                AddDeltaPosition_Procedural(delta);
                AddDeltaPosition_Animated(delta);
            }
		}
	}
}