using GAS.Runtime.Component;

namespace GAS.Runtime.Ability.TimelineAbility
{
    public class TimelineAbility:AbstractAbility
    {
        public readonly TimelineAbilityAsset AbilityAsset;
        public TimelineAbility(AbilityAsset abilityAsset) : base(abilityAsset)
        {
            AbilityAsset = abilityAsset as TimelineAbilityAsset;
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new TimelineAbilitySpec(this, owner);
        }
    }
    
    public class TimelineAbilitySpec: AbilitySpec
    {
        private TimelineAbility _ability;
        public TimelineAbilityAsset AbilityAsset => _ability.AbilityAsset;
        private TimelineAbilityPlayer _player;
        public TimelineAbilitySpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _ability = ability as TimelineAbility;
            _player = new TimelineAbilityPlayer(this);
        }

        public override void ActivateAbility(params object[] args)
        {
            // 播放时间轴
            _player.Play();
        }

        public override void CancelAbility()
        {
            // TODO 取消时间轴播放
            _player.Stop();
        }

        public override void EndAbility()
        {
            _player.Stop();
        }

        protected override void AbilityTick()
        {
            _player.Tick();
        }
    }
}