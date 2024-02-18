using GAS.Runtime.Component;

namespace GAS.Runtime.Ability
{
    public class GeneralSequentialAbility:AbstractAbility
    {
        public GeneralSequentialAbility(AbilityAsset abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new GeneralSequentialAbilitySpec(this, owner);
        }
    }
    
    public class GeneralSequentialAbilitySpec: AbilitySpec
    {
        private GeneralSequentialAbility _ability;
        private AbilitySystemComponent _owner;
        public GeneralSequentialAbilitySpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _ability = ability as GeneralSequentialAbility;
            _owner = owner;
        }

        public override void ActivateAbility(params object[] args)
        {
            // DoDodge().Forget();
        }

        public override void CancelAbility()
        {
            
        }

        public override void EndAbility()
        {
            
        }
    }
}