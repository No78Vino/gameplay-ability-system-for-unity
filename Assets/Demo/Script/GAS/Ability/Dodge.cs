using GAS.Runtime.Component;

namespace GAS.Runtime.Ability
{
    public class Dodge:AbstractAbility<AADodge>
    {
        public Dodge(AbilityAsset abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new DodgeSpec(this, owner);
        }
    }
    
    public class DodgeSpec: AbilitySpec
    {
        public DodgeSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
        }

        public override void ActivateAbility(params object[] args)
        {
            throw new System.NotImplementedException();
        }

        public override void CancelAbility()
        {
            throw new System.NotImplementedException();
        }

        public override void EndAbility()
        {
            throw new System.NotImplementedException();
        }
    }
}