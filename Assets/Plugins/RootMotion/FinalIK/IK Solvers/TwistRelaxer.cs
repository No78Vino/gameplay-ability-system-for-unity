using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK
{

    /// <summary>
    /// Relaxes the twist rotation if the TwistSolver transforms relative to their parent and a child Transforms, using their initial rotations as the most relaxed pose.
    /// </summary>
    public class TwistRelaxer : MonoBehaviour
    {

        public IK ik;

        [Tooltip("If using multiple solvers, add them in inverse hierarchical order - first forearm roll bone, then forearm bone and upper arm bone.")]
        public TwistSolver[] twistSolvers = new TwistSolver[0];

        public void Start()
        {
            if (twistSolvers.Length == 0)
            {
                Debug.LogError("TwistRelaxer has no TwistSolvers. TwistRelaxer.cs was restructured for FIK v2.0 to support multiple relaxers on the same body part and TwistRelaxer components need to be set up again, sorry for the inconvenience!", transform);
                return;
            }

            foreach (TwistSolver twistSolver in twistSolvers)
            {
                twistSolver.Initiate();
            }

            if (ik != null)
            {
                ik.GetIKSolver().OnPostUpdate += OnPostUpdate;
            }
        }

        private void Update()
        {
            if (ik != null && ik.fixTransforms)
            {
                foreach (TwistSolver twistSolver in twistSolvers)
                {
                    twistSolver.FixTransforms();
                }
            }
        }

        void OnPostUpdate()
        {
            if (ik != null)
            {
                foreach (TwistSolver twistSolver in twistSolvers)
                {
                    twistSolver.Relax();
                }
            }
        }

        void LateUpdate()
        {
            if (ik == null)
            {
                foreach (TwistSolver twistSolver in twistSolvers)
                {
                    twistSolver.Relax();
                }
            }
        }

        void OnDestroy()
        {
            if (ik != null) ik.GetIKSolver().OnPostUpdate -= OnPostUpdate;
        }
    }
}
