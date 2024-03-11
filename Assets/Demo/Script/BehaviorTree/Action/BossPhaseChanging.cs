using BehaviorDesigner.Runtime.Tasks;
using Demo.Script.Element;

namespace BehaviorDesigner.Runtime
{
    public class BossPhaseChanging:FightUnitActionBase
    {
        public override TaskStatus OnUpdate()
        {
            BossBladeFang boss = _unit.Value as BossBladeFang;
            return boss.ChangingPhase ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}