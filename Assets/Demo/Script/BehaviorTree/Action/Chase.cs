using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    /// <summary>
    /// 如果超过时间还没追到目标，返回失败
    /// </summary>
    public class Chase:FightUnitActionBase
    {
        [Tasks.Tooltip("The amount of time to wait")]
        public SharedFloat waitTime = 1;
        
        private float waitDuration;
        private float startTime;
        private float pauseTime;

        public override void OnStart()
        {
            startTime = Time.time;
            waitDuration = waitTime.Value;
        }

        public override TaskStatus OnUpdate()
        {
            if (_unit.Value.target == null) return TaskStatus.Failure;

            float direction = _unit.Value.target.transform.position.x - _unit.Value.transform.position.x;
            direction = direction > 0 ? 1 : -1;
            _unit.Value.ActivateMove(direction);
            if (_unit.Value.CatchTarget() || startTime + waitDuration < Time.time)
            {
                _unit.Value.DeactivateMove();
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }
    }
}