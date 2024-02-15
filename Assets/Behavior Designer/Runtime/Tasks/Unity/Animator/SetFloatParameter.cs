using UnityEngine;
using System.Collections;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator
{
    [TaskCategory("Unity/Animator")]
    [TaskDescription("Sets the float parameter on an animator. Returns Success.")]
    public class SetFloatParameter : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the parameter")]
        public SharedString paramaterName;
        [Tooltip("The value of the float parameter")]
        public SharedFloat floatValue;
        [Tooltip("Should the value be reverted back to its original value after it has been set?")]
        public bool setOnce;

        private int hashID;
        private Animator animator;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                animator = currentGameObject.GetComponent<Animator>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (animator == null) {
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }

            hashID = UnityEngine.Animator.StringToHash(paramaterName.Value);

            float prevValue = animator.GetFloat(hashID);
            animator.SetFloat(hashID, floatValue.Value);
            if (setOnce) {
                StartCoroutine(ResetValue(prevValue));
            }

            return TaskStatus.Success;
        }

        public IEnumerator ResetValue(float origVale)
        {
            yield return null;
            animator.SetFloat(hashID, origVale);
        }

        public override void OnReset()
        {
            targetGameObject = null;
            paramaterName = "";
            floatValue = 0;
        }
    }
}