using BehaviorDesigner.Runtime.Tasks;
using GAS.Editor;

namespace BehaviorDesigner.Runtime
{
    public class WaitForAttackEnd:FightUnitActionBase
    {
        public override TaskStatus OnUpdate()
        {
            bool inAttack = _unit.Value.ASC.HasTag(GTagLib.Event_Attacking);
            return inAttack ? TaskStatus.Running : TaskStatus.Success;
        }
    }
}