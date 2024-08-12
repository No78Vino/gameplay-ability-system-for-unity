using UnityEngine;

namespace GAS.Runtime
{
    public class Move : AbstractAbility<AAMove>
    {
        public sealed record Args(float Direction);

        public Move(AAMove abilityAsset) : base(abilityAsset)
        {
        }

        public override AbilitySpec CreateSpec(AbilitySystemComponent owner)
        {
            return new MoveSpec(this, owner);
        }
    }

    public class MoveSpec : AbilitySpec
    {
        private FightUnit _unit;
        private Move.Args _moveArgs;

        public MoveSpec(AbstractAbility ability, AbilitySystemComponent owner) : base(ability, owner)
        {
            _unit = owner.GetComponent<FightUnit>();
        }

        public override void ActivateAbility(object arg = null, GameplayEffectSpec gameplayEffectSpec = null)
        {
            if (arg is not Move.Args moveArgs)
                throw new System.Exception("arg is not Move.Args");
            _moveArgs = moveArgs;

            // var direction = moveArgs.Direction;
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

            // 切换状态至Idle
            PlayMoveAnimation();

            float direction = _moveArgs.Direction;
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

        private void PlayMoveAnimation()
        {
            AnimatorStateInfo currentState = _unit.Animator.GetCurrentAnimatorStateInfo(0);

            string stateName = "Idle";
            if (currentState.IsName(stateName)) return;

            _unit.Animator.Play(stateName, 0, 0f);
        }
    }
}