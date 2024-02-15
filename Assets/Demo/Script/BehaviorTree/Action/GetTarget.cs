using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
    public class GetTarget:FightUnitActionBase
    {
        public override TaskStatus OnUpdate()
        {
            return _unit.Value.CatchTarget() ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}