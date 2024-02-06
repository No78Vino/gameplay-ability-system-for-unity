using GAS.Runtime.Component;

namespace GAS.Runtime.Ability
{
    public class Defend:AbstractAbility<AADefend>
    {
        public Defend(AbilityAsset abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new DefendSpec(this, owner);
        }
    }
    
    public class DefendSpec: AbilitySpec
    {
        public DefendSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
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