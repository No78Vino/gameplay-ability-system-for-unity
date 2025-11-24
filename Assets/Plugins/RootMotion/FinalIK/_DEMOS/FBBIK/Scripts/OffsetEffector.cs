using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

namespace RootMotion.Demos {

	/// <summary>
	/// Custom positionOffset effector for FBBIK, could be used for example to make a spine or pelvis effector.
	/// </summary>
	public class OffsetEffector : OffsetModifier {

		[System.Serializable]
		public class EffectorLink {
			public FullBodyBipedEffector effectorType;
			public float weightMultiplier = 1f;

			[HideInInspector] public Vector3 localPosition;
		}

        [Tooltip("Optional. Assign the bone Transform that is closest to this OffsetEffector to be able to call OffsetEffector.Anchor() in LateUpdate to match its position and rotation to animation.")]
        public Transform anchor;
		public EffectorLink[] effectorLinks;

        private Vector3 posRelToAnchor;
        private Quaternion rotRelToAnchor = Quaternion.identity;

		protected override void Start() {
			base.Start();

            if (anchor != null)
            {
                posRelToAnchor = anchor.InverseTransformPoint(transform.position);
                rotRelToAnchor = Quaternion.Inverse(anchor.rotation) * transform.rotation;
            }

			// Store the default positions of the effectors relative to this GameObject's position
			foreach (EffectorLink e in effectorLinks) {
                var bone = ik.solver.GetEffector(e.effectorType).bone;
                e.localPosition = transform.InverseTransformPoint(bone.position);
                if (e.effectorType == FullBodyBipedEffector.Body) ik.solver.bodyEffector.effectChildNodes = false;
			}
		}

		protected override void OnModifyOffset() {
            // Update the effectors
			foreach (EffectorLink e in effectorLinks) {
				// Using effector positionOffset
				Vector3 positionTarget = transform.TransformPoint(e.localPosition);

				ik.solver.GetEffector(e.effectorType).positionOffset += (positionTarget - (ik.solver.GetEffector(e.effectorType).bone.position + ik.solver.GetEffector(e.effectorType).positionOffset)) * weight * e.weightMultiplier;
			}
		}

        public void Anchor()
        {
            if (anchor == null) return;

            transform.position = anchor.TransformPoint(posRelToAnchor);
            transform.rotation = anchor.rotation * rotRelToAnchor;
        }
    }
}
