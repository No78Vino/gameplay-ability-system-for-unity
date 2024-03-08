using Demo.Script.Element;
using GAS.Runtime.Ability;

namespace Demo.Script.GAS.AbilityTask
{
    public class TeleportAbovePlayer:InstantAbilityTask
    {
        public override void OnExecute()
        {
            var boss = _spec.Owner.GetComponent<FightUnit>() as BossBladeFang;
            boss.transform.position = boss.target.transform.position + new UnityEngine.Vector3(0, 6, 0);
        }
    }
}