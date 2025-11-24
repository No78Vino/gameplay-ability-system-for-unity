using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

namespace RootMotion.FinalIK
{
    /// <summary>
    /// Bend goal object for FABRIK. Add this to a GameObject you wish FABRIK to bend towards.
    /// </summary>
    public class FABRIKBendGoal : MonoBehaviour
    {

        public FABRIK ik;
        [Range(0f, 1f)] public float weight = 1f;
        
        private void Start()
        {
            ik.solver.OnPreIteration += OnPreIteration;
        }

        void OnPreIteration(int it)
        {
            if (it != 0) return;
            if (weight <= 0f) return;

            Vector3 bendDirection = transform.position - ik.solver.bones[0].transform.position;
            bendDirection *= weight;

            foreach (IKSolverFABRIK.Bone bone in ik.solver.bones)
            {
                bone.solverPosition += bendDirection;
            }
        }

        private void OnDestroy()
        {
            if (ik != null) ik.solver.OnPreIteration -= OnPreIteration;
        }
    }
}
