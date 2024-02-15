using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityString
{
    [TaskCategory("Unity/String")]
    [TaskDescription("Randomly selects a string from the array of strings.")]
    public class GetRandomString : Action
    {
        [Tooltip("The array of strings")]
        public SharedString[] source;
        [Tooltip("The stored result")]
        [RequiredField]
        public SharedString storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = source[Random.Range(0, source.Length)].Value;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            source = null;
            storeResult = null;
        }
    }
}