using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

namespace RootMotion.Demos {

	public class AimSwing : MonoBehaviour {

		public AimIK ik;
		[Tooltip("The direction in which the weapon is aimed in animation (in character space). Tweak this value to adjust the aiming.")] public Vector3 animatedAimDirection = Vector3.forward;

		void LateUpdate () {
			ik.solver.axis = ik.solver.transform.InverseTransformVector(ik.transform.rotation * animatedAimDirection);
		}
	}
}
