using UnityEngine;
using RootMotion.FinalIK;

namespace RootMotion.Demos
{
    /// <summary>
    /// Makes VRIK work with elbow targets at a cost of reduced hand accuracy.
    /// </summary>
    public class VRIKArmMocap : MonoBehaviour
    {

        public VRIK ik;
        public Transform leftElbowTarget;
        public Transform rightElbowTarget;

        void Start()
        {
            // Register to get a call from VRIK after it updates
            ik.solver.OnPostUpdate += AfterVRIK;
        }

        void AfterVRIK()
        {
            // This is called by VRIK each time it is done updating for the frame
            UpdateArm(ik.references.leftUpperArm, ik.references.leftForearm, ik.references.leftHand, leftElbowTarget, ik.solver.leftArm.target);
            UpdateArm(ik.references.rightUpperArm, ik.references.rightForearm, ik.references.rightHand, rightElbowTarget, ik.solver.rightArm.target);
        }

        private static void UpdateArm(Transform upperArm, Transform forearm, Transform hand, Transform elbowTarget, Transform handTarget)
        {
            if (elbowTarget == null) return;
            if (handTarget == null) return;

            // Rotate the upper arm towards the elbow target.
            upperArm.rotation = Quaternion.FromToRotation(forearm.position - upperArm.position, elbowTarget.position - upperArm.position) * upperArm.rotation;

            // Rotate the forearm towards the hand target
            forearm.rotation = Quaternion.FromToRotation(hand.position - forearm.position, handTarget.position - forearm.position) * forearm.rotation;
        }

        void OnDestroy()
        {
            // Clean up
            if (ik != null) ik.solver.OnPostUpdate -= AfterVRIK;
        }
    }
}
