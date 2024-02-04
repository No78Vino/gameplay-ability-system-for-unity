using GAS.Runtime.Component;

namespace GAS.Runtime.Ability
{
    public class Jump:AbstractAbility<AAJump>
    {
        public readonly float JumpPower;
        public Jump(AbilityAsset abilityAsset) : base(abilityAsset)
        {
            JumpPower = AbilityAsset.JumpPower;
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new JumpSpec(this, owner);
        }
    }
    
    public class JumpSpec: AbilitySpec
    {
        public JumpSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
        }

        public override void ActivateAbility(params object[] args)
        {
            // var velocity = _rb.velocity;
            // velocity.y = jumpVelocity;
            // _rb.velocity = velocity;
            //     
            TryEndAbility();
        }

        public override void CancelAbility()
        {
            
        }

        public override void EndAbility()
        {
        }
    }
}