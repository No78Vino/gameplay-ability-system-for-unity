using GAS.RuntimeWithECS.Ability.Component;
using Unity.Entities;

namespace GAS.Runtime
{
    
    public class ALTimeline: AbilityLogicBase<AbilityParamTimeline>
    {
        protected ALTimelinePlayer _player;

        /// <summary>
        /// 向性技能的作用目标
        /// </summary>
        public Entity Target { get; private set; }

        public ALTimeline(Entity ability) : base(ability)
        {
            _player = new ALTimelinePlayer(this);
        }

        public AbilityParamTimeline GetParam()
        {
            return _param;
        }
        
        public void SetAbilityTarget(Entity mainTarget)
        {
            Target = mainTarget;
        }
        
        public override void ActivateAbility(GlobalTimer timer)
        {
            _player.Play();
        }

        public override void CancelAbility(GlobalTimer timer)
        {
            _player.Stop();
        }

        public override void EndAbility(GlobalTimer timer)
        {
            _player.Stop();
        }

        public override void AbilityTick(GlobalTimer timer)
        {
            _player.Tick();
        }
    }
}