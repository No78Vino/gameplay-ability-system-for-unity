using BehaviorDesigner.Runtime.Tasks;
using Demo.Script.Element;

namespace BehaviorDesigner.Runtime
{
    public class BossPhaseChangeEnd:FightUnitActionBase
    {
        public override TaskStatus OnUpdate()
        {
            BossBladeFang boss = _unit.Value as BossBladeFang;
            boss.ChangingPhaseEnd();
            return TaskStatus.Success;
        }
    }
}