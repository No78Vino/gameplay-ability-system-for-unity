using GAS.Runtime;

namespace GAS.Runtime
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

        /// <summary>
        /// 向性技能的作用目标
        /// </summary>
        public AbilitySystemComponent Target { get; private set; }

        public TimelineAbilitySpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _ability = ability as TimelineAbility;
            _player = new TimelineAbilityPlayer(this);
        }

        public void SetAbilityTarget(AbilitySystemComponent mainTarget)
        {
            Target = mainTarget;
        }
        
        public override void ActivateAbility(params object[] args)
        {
             _player.Play();
        }

        public override void CancelAbility() 
        {
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