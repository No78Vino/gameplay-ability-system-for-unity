using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
    public class Chase:FightUnitActionBase
    {
        public override TaskStatus OnUpdate()
        {
            if(_unit.Value.target == null)
                return TaskStatus.Failure;
            
            float direction = _unit.Value.target.transform.position.x - _unit.Value.transform.position.x;
            direction = direction > 0 ? 1 : -1;
            _unit.Value.ActivateMove(direction);
            if (_unit.Value.CatchTarget())
            {
                _unit.Value.DeactivateMove();
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}