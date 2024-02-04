using GAS.Runtime.Component;

namespace GAS.Runtime.Ability
{
    public class Move:AbstractAbility<AAMove>
    {
        public Move(AbilityAsset abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new MoveSpec(this, owner);
        }
    }
    
    public class MoveSpec: AbilitySpec
    {
        public MoveSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
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