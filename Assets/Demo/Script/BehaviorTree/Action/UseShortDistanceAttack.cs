using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime
{
    public class UseShortDistanceAttack:FightUnitActionBase
    {
        public float ShortDistance = 6;
        
        public override TaskStatus OnUpdate()
        {
            float distance = _unit.Value.target.transform.position.x - _unit.Value.transform.position.x;
            distance = distance < 0 ? -distance : distance;
            return distance <= ShortDistance ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}