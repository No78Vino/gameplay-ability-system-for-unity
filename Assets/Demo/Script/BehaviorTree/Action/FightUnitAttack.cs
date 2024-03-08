using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    public class FightUnitAttack:FightUnitActionBase
    {
        public override TaskStatus OnUpdate()
        {
            if (_unit.Value.target == null)
                return TaskStatus.Failure;
            // 朝向目标
            _unit.Value.transform.localScale =
                new Vector3(_unit.Value.target.transform.position.x < _unit.Value.transform.position.x ? -1 : 1, 1, 1);
            return _unit.Value.Attack() ? TaskStatus.Success :TaskStatus.Failure;
        }
    }
}