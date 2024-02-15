using GAS.Runtime.Component;
using UnityEngine;

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
        private FightUnit _unit;
        public MoveSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _unit = owner.GetComponent<FightUnit>();
        }

        public override void ActivateAbility(params object[] args)
        {
            // float direction = (float) _abilityArguments[0];
            // if (Mathf.Abs(direction) > 0)
            // {
            //     _unit.SetVelocityX(direction > 0 ? 1 : -1);
            //     _unit.Renderer.localScale = new Vector3(_unit.VelocityX, 1, 1);
            // }
            // else
            // {
            //     _unit.SetVelocityX(0);
            // }
        }

        public override void CancelAbility()
        {
            _unit.SetVelocityX(0);
        }

        public override void EndAbility()
        {
            CancelAbility();
        }

        protected override void AbilityTick()
        {
            base.AbilityTick();
            float direction = (float) _abilityArguments[0];
            if (Mathf.Abs(direction) > 0)
            {
                _unit.SetVelocityX(direction > 0 ? 1 : -1);
                _unit.Renderer.localScale = new Vector3(_unit.VelocityX, 1, 1);
            }
            else
            {
                _unit.SetVelocityX(0);
            }
        }
    }
}