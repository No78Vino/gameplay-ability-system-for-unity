using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.FinalIK
{
    /// <summary>
    /// Relaxes the twist rotation if the Transform relative to its parent and a child Transforms, using the Transform's initial rotation as the most relaxed pose.
    /// </summary>
    [System.Serializable]
    public class TwistSolver
    {
        [Tooltip("The transform that this solver operates on.")]
        public Transform transform;

        [Tooltip("If this is the forearm roll bone, the parent should be the forearm bone. If null, will be found automatically.")]
        public Transform parent;

        [Tooltip("If this is the forearm roll bone, the child should be the hand bone. If null, will attempt to find automatically. Assign the hand manually if the hand bone is not a child of the roll bone.")]
        public Transform[] children = new Transform[0];

        [Tooltip("The weight of relaxing the twist of this Transform")]
        [Range(0f, 1f)] public float weight = 1f;

        [Tooltip("If 0.5, this Transform will be twisted half way from parent to child. If 1, the twist angle will be locked to the child and will rotate with along with it.")]
        [Range(0f, 1f)] public float parentChildCrossfade = 0.5f;

        [Tooltip("Rotation offset around the twist axis.")]
        [Range(-180f, 180f)] public float twistAngleOffset;

        private Vector3 twistAxis = Vector3.right;
        private Vector3 axis = Vector3.forward;
        private Vector3 axisRelativeToParentDefault, axisRelativeToChildDefault;
        private Quaternion[] childRotations;
        private bool inititated;
        private Quaternion defaultLocalRotation = Quaternion.identity;
        private Quaternion[] defaultChildLocalRotations;

        public TwistSolver()
        {
            weight = 1f;
            parentChildCrossfade = 0.5f;
        }

        public TwistSolver(Transform t)
        {
            transform = t;
            weight = 1f;
            parentChildCrossfade = 0.5f;
        }

        /// <summary>
        /// Initiate this TwistSolver
        /// </summary>
        public void Initiate()
        {
            if (inititated) return;

            if (transform == null)
            {
                Debug.LogError("TwistRelaxer solver has unassigned Transform. TwistRelaxer.cs was restructured for FIK v2.0 to support multiple relaxers on the same body part and TwistRelaxer components need to be set up again, sorry for the inconvenience!", transform);
                return;
            }

            if (parent == null) parent = transform.parent;

            if (children.Length == 0)
            {
                if (transform.childCount == 0)
                {
                    var children = parent.GetComponentsInChildren<Transform>();
                    for (int i = 1; i < children.Length; i++)
                    {
                        if (children[i] != transform)
                        {
                            children = new Transform[1] { children[i] };
                            break;
                        }
                    }
                }
                else
                {
                    children = new Transform[1] { transform.GetChild(0) };
                }
            }

            if (children.Length == 0 || children[0] == null)
            {
                Debug.LogError("TwistRelaxer has no children assigned.", transform);
                return;
            }

            twistAxis = transform.InverseTransformDirection(children[0].position - transform.position);
            axis = new Vector3(twistAxis.y, twistAxis.z, twistAxis.x);

            // Axis in world space
            Vector3 axisWorld = transform.rotation * axis;

            // Store the axis in worldspace relative to the rotations of the parent and child
            axisRelativeToParentDefault = Quaternion.Inverse(parent.rotation) * axisWorld;
            axisRelativeToChildDefault = Quaternion.Inverse(children[0].rotation) * axisWorld;

            childRotations = new Quaternion[children.Length];

            defaultLocalRotation = transform.localRotation;
            defaultChildLocalRotations = new Quaternion[children.Length];
            for (int i = 0; i < children.Length; i++)
            {
                defaultChildLocalRotations[i] = children[i].localRotation;
            }

            //if (ik != null) ik.GetIKSolver().OnPostUpdate += OnPostUpdate;
            inititated = true;
        }

        /// <summary>
        /// Rotates the bone back to default localRotation.
        /// </summary>
        public void FixTransforms()
        {
            transform.localRotation = defaultLocalRotation;

            for (int i = 0; i < children.Length; i++)
            {
                children[i].localRotation = defaultChildLocalRotations[i];
            }
        }

        /// <summary>
        /// Rotate this Transform to relax its twist angle relative to the "parent" and "child" Transforms.
        /// </summary>
        public void Relax()
        {
            if (!inititated) return;
            if (weight <= 0f) return; // Nothing to do here

            Quaternion rotation = transform.rotation;
            Quaternion twistOffset = Quaternion.AngleAxis(twistAngleOffset, rotation * twistAxis);
            rotation = twistOffset * rotation;

            // Find the world space relaxed axes of the parent and child
            Vector3 relaxedAxisParent = twistOffset * parent.rotation * axisRelativeToParentDefault;
            Quaternion f = Quaternion.FromToRotation(transform.position - parent.position, children[0].position - transform.position);
            relaxedAxisParent = f * relaxedAxisParent;

            Vector3 relaxedAxisChild = twistOffset * children[0].rotation * axisRelativeToChildDefault;

            // Cross-fade between the parent and child
            Vector3 relaxedAxis = Vector3.Slerp(relaxedAxisParent, relaxedAxisChild, parentChildCrossfade);

            // Convert relaxedAxis to (axis, twistAxis) space so we could calculate the twist angle
            Quaternion r = Quaternion.LookRotation(rotation * axis, rotation * twistAxis);
            relaxedAxis = Quaternion.Inverse(r) * relaxedAxis;

            // Calculate the angle by which we need to rotate this Transform around the twist axis.
            float angle = Mathf.Atan2(relaxedAxis.x, relaxedAxis.z) * Mathf.Rad2Deg;

            // Store the rotation of the child so it would not change with twisting this Transform
            for (int i = 0; i < children.Length; i++)
            {
                childRotations[i] = children[i].rotation;
            }

            // Twist the bone
            transform.rotation = Quaternion.AngleAxis(angle * weight, rotation * twistAxis) * rotation;

            // Revert the rotation of the child
            for (int i = 0; i < children.Length; i++)
            {
                children[i].rotation = childRotations[i];
            }
        }
    }
}
