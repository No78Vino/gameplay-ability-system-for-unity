using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.FinalIK
{
    [CreateAssetMenu(fileName = "Editor IK Pose", menuName = "Final IK/Editor IK Pose", order = 1)]
    public class EditorIKPose : ScriptableObject
    {

        public Vector3[] localPositions = new Vector3[0];
        public Quaternion[] localRotations = new Quaternion[0];

        public bool poseStored
        {
            get
            {
                return localPositions.Length > 0;
            }
        }

        public void Store(Transform[] T)
        {
            localPositions = new Vector3[T.Length];
            localRotations = new Quaternion[T.Length];

            for (int i = 1; i < T.Length; i++)
            {
                localPositions[i] = T[i].localPosition;
                localRotations[i] = T[i].localRotation;
            }
        }

        public bool Restore(Transform[] T)
        {
            if (localPositions.Length != T.Length)
            {
                Debug.LogError("Can not restore pose (unmatched bone count). Please stop the solver and click on 'Store Default Pose' if you have made changes to character hierarchy.");
                return false;
            }

            for (int i = 1; i < T.Length; i++)
            {
                T[i].localPosition = localPositions[i];
                T[i].localRotation = localRotations[i];
            }

            return true;
        }
    }
}
