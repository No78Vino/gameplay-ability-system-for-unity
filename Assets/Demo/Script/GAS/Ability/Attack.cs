using GAS.Runtime.Component;

namespace GAS.Runtime.Ability
{
    public class Attack:AbstractAbility<AAAttack>
    {
        public Attack(AbilityAsset abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new AttackSpec(this, owner);
        }
    }
    
    public class AttackSpec: AbilitySpec
    {
        public AttackSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
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