using Unity.Entities;

namespace GAS.Runtime
{
    public class ALTimeline: AbilityLogicBase<XParamALTimelineID>
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

        public override void SetParam(XParam abilityParam)
        {
            base.SetParam(abilityParam);
            _player?.InitData();
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