using GAS.Runtime;

namespace GAS.Runtime
{
    public class Jump:AbstractAbility<AAJump>
    {
        public readonly float JumpPower;
        public Jump(AAJump abilityAsset) : base(abilityAsset)
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
        Jump _jump;
        public JumpSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _jump = ability as Jump;
        }

        public override void ActivateAbility(params object[] args)
        {
            var rb = args[0] as UnityEngine.Rigidbody2D;
            var velocity = rb.velocity;
            velocity.y = _jump.JumpPower;
            rb.velocity = velocity;
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