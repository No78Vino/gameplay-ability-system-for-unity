using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
    public class Die:FightUnitActionBase
    {
        public override TaskStatus OnUpdate()
        {
            _unit.Value.Die();
            return TaskStatus.Success;
        }
    }
}