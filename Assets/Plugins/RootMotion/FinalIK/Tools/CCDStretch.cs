using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.FinalIK
{

    /// <summary>
    /// Automatic stretch and squash for CCDIK.
    /// </summary>
    public class CCDStretch : MonoBehaviour
    {

        public CCDIK ik;
        [Range(0f, 0.999f)] public float maxSquash = 0f;
        public float maxStretch = 2f;

        private Vector3[] defaultLocalPositions = new Vector3[0];

        private void Start()
        {
            // Store default localPositions
            defaultLocalPositions = new Vector3[ik.solver.bones.Length - 1];
            for (int i = 1; i < ik.solver.bones.Length; i++)
            {
                defaultLocalPositions[i - 1] = ik.solver.bones[i].transform.localPosition;
            }
        }

        private void LateUpdate()
        {
            // Reset to default localPositions
            for (int i = 1; i < ik.solver.bones.Length; i++)
            {
                ik.solver.bones[i].transform.localPosition = defaultLocalPositions[i - 1];
            }

            // Get distance from first bone to target
            float targetDist = Vector3.Magnitude((ik.solver.target != null ? ik.solver.target.position : ik.solver.IKPosition) - ik.solver.bones[0].transform.position);

            // Get bone chain length
            float chainLength = 0f;
            for (int i = 1; i < ik.solver.bones.Length; i++)
            {
                chainLength += Vector3.Magnitude(ik.solver.bones[i].transform.position - ik.solver.bones[i - 1].transform.position);
            }

            // Compare target distance to chain length...
            maxStretch = Mathf.Max(maxStretch, 1f);
            float mlp = Mathf.Clamp(targetDist / chainLength, 1f - maxSquash, maxStretch);

            // Stretch
            for (int i = 1; i < ik.solver.bones.Length; i++)
            {
                ik.solver.bones[i].transform.localPosition *= mlp;
            }
        }
    }
}
