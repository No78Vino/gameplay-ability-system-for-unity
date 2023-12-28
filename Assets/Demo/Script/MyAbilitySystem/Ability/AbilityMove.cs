using Demo.Script.MyAbilitySystem.AbilityTask;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.AbilityTask;
using GAS.Runtime.Component;

namespace Demo.Script.MyAbilitySystem.Ability
{
    public class AbilityMove:AbstractAbility
    {
        
        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new AbilitySpecMove(this, owner);
        }

        public AbilityMove(AbilityAsset abilityAsset) : base(abilityAsset)
        {
        }
    }
    
    public class AbilitySpecMove:AbilitySpec
    {

        public AbilitySpecMove(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
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