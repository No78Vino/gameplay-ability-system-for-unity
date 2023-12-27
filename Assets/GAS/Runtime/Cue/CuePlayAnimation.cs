using GAS.Runtime.Component;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    public class CuePlayAnimation : GameplayCue
    {
        private readonly string _animationName;
        private readonly Animator _animator;
        private AbilitySystemComponent _target;

        public CuePlayAnimation(AbilitySystemComponent target, string animationName)
        {
            _target = target;
            _animator = target.GetComponent<Animator>();
            _animationName = animationName;
        }

        public override void Trigger()
        {
            _animator.Play(_animationName);
        }
    }
}