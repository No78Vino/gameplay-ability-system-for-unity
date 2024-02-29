using GAS.Runtime.Component;

namespace GAS.Runtime.Ability.TimelineAbility
{
    public class TimelineAbility:AbstractAbility
    {
        public TimelineAbility(AbilityAsset abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new TimelineAbilitySpec(this, owner);
        }
    }
    
    public class TimelineAbilitySpec: AbilitySpec
    {
        private TimelineAbility _ability;
        private AbilitySystemComponent _owner;
        public TimelineAbilitySpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _ability = ability as TimelineAbility;
            _owner = owner;
        }

        public override void ActivateAbility(params object[] args)
        {
        }

        public override void CancelAbility()
        {
            
        }

        public override void EndAbility()
        {
            
        }
    }
}