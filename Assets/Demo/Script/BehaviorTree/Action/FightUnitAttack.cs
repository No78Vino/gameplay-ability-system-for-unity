using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
    public class FightUnitAttack:FightUnitActionBase
    {
        public override TaskStatus OnUpdate()
        {
            if (_unit.Value.target == null)
                return TaskStatus.Failure;
            return _unit.Value.Attack() ? TaskStatus.Success :TaskStatus.Failure;
        }
    }
}