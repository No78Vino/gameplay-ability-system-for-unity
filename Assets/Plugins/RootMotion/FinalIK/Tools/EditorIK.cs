using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

namespace RootMotion.FinalIK
{
    /// <summary>
    /// Updates any Final IK component in Editor mode
    /// </summary>
    [ExecuteInEditMode]
    public class EditorIK : MonoBehaviour
    {
        [Tooltip("If slot assigned, will evaluate PlayableDirector before solving the IK.")] public PlayableDirector playableDirector;
        [Tooltip("If slot assigned, will update Animator before solving the IK.")] public Animator animator;
        [Tooltip("Create/Final IK/Editor IK Pose")] public EditorIKPose defaultPose;

        [HideInInspector] public Transform[] bones = new Transform[0];
        
        public IK ik { get; private set; }

        private void OnEnable()
        {
            if (Application.isPlaying) return;
            if (ik == null) ik = GetComponent<IK>();
            if (ik == null)
            {
                Debug.LogError("EditorIK needs to have an IK component on the same GameObject.", transform);
                return;
            }
            if (bones.Length == 0) bones = ik.transform.GetComponentsInChildren<Transform>();
        }

        private void OnDisable()
        {
            if (Application.isPlaying) return;
            if (defaultPose != null && defaultPose.poseStored) defaultPose.Restore(bones);
            if (ik != null) ik.GetIKSolver().executedInEditor = false;
        }

        private void OnDestroy()
        {
            if (Application.isPlaying) return;
            if (ik == null) return;
            if (bones.Length == 0) bones = ik.transform.GetComponentsInChildren<Transform>();
            if (defaultPose != null && defaultPose.poseStored && bones.Length != 0) defaultPose.Restore(bones);
            ik.GetIKSolver().executedInEditor = false;
        }

        public void StoreDefaultPose()
        {
            bones = ik.transform.GetComponentsInChildren<Transform>();
            defaultPose.Store(bones);
        }

        public bool Initiate()
        {
            if (defaultPose == null) return false;
            if (!defaultPose.poseStored) return false;
            if (bones.Length == 0) return false;

            if (ik == null) ik = GetComponent<IK>();
            if (ik == null)
            {
                Debug.LogError("EditorIK can not find an IK component.", transform);
                return false;
            }

            defaultPose.Restore(bones);
            
            ik.GetIKSolver().executedInEditor = false;
            ik.GetIKSolver().Initiate(ik.transform);
            ik.GetIKSolver().executedInEditor = true;
            return true;
        }

        public void Update()
        {
            if (Application.isPlaying) return;
            if (ik == null) return;
            if (!ik.enabled) return;
            if (!ik.GetIKSolver().executedInEditor) return;
            if (bones.Length == 0) bones = ik.transform.GetComponentsInChildren<Transform>();
            if (bones.Length == 0) return;

            if (!defaultPose.Restore(bones)) return;

            ik.GetIKSolver().executedInEditor = false;
            if (!ik.GetIKSolver().initiated) ik.GetIKSolver().Initiate(ik.transform);
            if (!ik.GetIKSolver().initiated) return;
            ik.GetIKSolver().executedInEditor = true;

            if (playableDirector != null)
            {
                playableDirector.Evaluate();
            }
            else
            {
                if (animator != null && animator.runtimeAnimatorController != null) animator.Update(Time.deltaTime);
            }

            ik.GetIKSolver().Update();
        }
    }
}