using BehaviorDesigner.Runtime.Tasks;
using Demo.Script.Element;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    public enum BossAttackType
    {
        Melee,
        FallDown,
        Beam,
        Roar
    }

    public class FightUnitAttack : FightUnitActionBase
    {
        public BossAttackType AttackType;

        public override TaskStatus OnUpdate()
        {
            if (_unit.Value.target == null)
                return TaskStatus.Failure;

            BossBladeFang boss = _unit.Value as BossBladeFang;
            if (AttackType == BossAttackType.Melee)
            {
                // 朝向目标
                boss.transform.localScale =
                    new Vector3(boss.target.transform.position.x < boss.transform.position.x ? -1 : 1, 1,
                        1);
                return boss.Attack() ? TaskStatus.Success : TaskStatus.Failure;
            }
            if (AttackType == BossAttackType.FallDown)
            {
                return boss.FallDownAttack() ? TaskStatus.Success : TaskStatus.Failure;
            }

            if (AttackType == BossAttackType.Beam)
            {
                return boss.BeamAttack() ? TaskStatus.Success : TaskStatus.Failure;
            }

            if (AttackType == BossAttackType.Roar)
            {
                return boss.RoarAttack() ? TaskStatus.Success : TaskStatus.Failure;
            }


            return TaskStatus.Failure;
        }
    }
}