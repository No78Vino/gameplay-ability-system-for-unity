using GAS.Runtime.Ability;
using GAS.Runtime.AbilitySystemComponent;

namespace Demo.Script.MyAbilitySystem.Ability
{
    public class AbilityMove:AbstractAbility
    {
        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            throw new System.NotImplementedException();
        }

        public override void ActivateAbility()
        {
            throw new System.NotImplementedException();
        }

        public override void EndAbility()
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class AbilitySpecMove:AbilitySpec
    {

        public override bool StepAbility()
        {
            throw new System.NotImplementedException();
        }

        public override bool CheckGameplayTags()
        {
            throw new System.NotImplementedException();
        }

        public override void EndAbility()
        {
            throw new System.NotImplementedException();
        }

        public AbilitySpecMove(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
        }
    }
}