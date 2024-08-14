using System;
using UnityEngine;

namespace GAS.Runtime
{
    public class Jump : AbstractAbility<AAJump>
    {
        public sealed record Args(Rigidbody2D Rigidbody2D);

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

    public class JumpSpec : AbilitySpec
    {
        Jump _jump;

        public JumpSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _jump = ability as Jump;
        }

        public override void ActivateAbility()
        {
            if (AbilityArgument is not Jump.Args jumpArgs)
                throw new Exception("arg is not Jump.JumpArgs");

            var rb = jumpArgs.Rigidbody2D;
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