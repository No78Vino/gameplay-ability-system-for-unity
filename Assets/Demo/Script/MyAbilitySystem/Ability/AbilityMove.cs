using Demo.Script.MyAbilitySystem.AbilityTask;
using GAS.Runtime.Ability;
using GAS.Runtime.Ability.AbilityTask;
using GAS.Runtime.AbilitySystemComponent;

namespace Demo.Script.MyAbilitySystem.Ability
{
    public class AbilityMove:AbstractAbility
    {
        public AbilityMove()
        {
            Name = "Move";
            OngoingAbilityTasks.Add(new OgATMove());
        }
        
        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new AbilitySpecMove(this, owner);
        }

        public override void ActivateAbility()
        {
            
        }

        public override void EndAbility()
        {
            
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