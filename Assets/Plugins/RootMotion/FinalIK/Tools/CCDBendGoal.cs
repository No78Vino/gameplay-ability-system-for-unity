using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.FinalIK
{

    /// <summary>
    /// Bend goal object for CCDIK. Add this to a GameObject you wish CCD to bend towards.
    /// </summary>
    public class CCDBendGoal : MonoBehaviour
    {

        public CCDIK ik;
        [Range(0f, 1f)] public float weight = 1f;

        private void Start()
        {
            ik.solver.OnPreUpdate += BeforeIK;
        }

        private void BeforeIK()
        {
            if (!enabled) return;
            float w = ik.solver.IKPositionWeight * weight;
            if (w <= 0f) return;

            Vector3 firstBonePos = ik.solver.bones[0].transform.position;
            Vector3 lastBonePos = ik.solver.bones[ik.solver.bones.Length - 1].transform.position;

            // Rotating the CCD chain towards this gameobject before it solves so it rolls in from that direction
            Quaternion f = Quaternion.FromToRotation(lastBonePos - firstBonePos, transform.position - firstBonePos);
            
            if (w < 1f) f = Quaternion.Slerp(Quaternion.identity, f, w);

            ik.solver.bones[0].transform.rotation = f * ik.solver.bones[0].transform.rotation;
        }

        private void OnDestroy()
        {
            if (ik != null) ik.solver.OnPreUpdate -= BeforeIK;
        }
    }
}
