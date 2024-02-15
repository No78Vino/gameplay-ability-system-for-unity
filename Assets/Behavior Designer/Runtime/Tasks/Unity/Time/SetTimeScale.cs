using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityTime
{
    [TaskCategory("Unity/Time")]
    [TaskDescription("Sets the scale at which time is passing.")]
    public class SetTimeScale : Action
    {
        [Tooltip("The timescale")]
        public SharedFloat timeScale;

        public override TaskStatus OnUpdate()
        {
            Time.timeScale = timeScale.Value;
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            timeScale.Value = 0;
        }
    }
}