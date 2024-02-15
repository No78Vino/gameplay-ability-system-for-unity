using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2
{
    [TaskCategory("Unity/Vector2")]
    [TaskDescription("Returns the distance between two Vector2s.")]
    public class Distance : Action
    {
        [Tooltip("The first Vector2")]
        public SharedVector2 firstVector2;
        [Tooltip("The second Vector2")]
        public SharedVector2 secondVector2;
        [Tooltip("The distance")]
        [RequiredField]
        public SharedFloat storeResult;

        public override TaskStatus OnUpdate()
        {
            storeResult.Value = Vector2.Distance(firstVector2.Value, secondVector2.Value);
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            firstVector2 = Vector2.zero; 
            secondVector2 = Vector2.zero;
            storeResult = 0;
        }
    }
}