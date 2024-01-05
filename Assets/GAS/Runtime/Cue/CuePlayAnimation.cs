using GAS.Runtime.Component;
using UnityEngine;

namespace GAS.Runtime.Cue
{
    [CreateAssetMenu(fileName = "CuePlayAnimation", menuName = "GAS/Cue/CuePlayAnimation")]
    public class CuePlayAnimation : GameplayCue
    {
        [SerializeField] private string _animationName;
        private Animator _animator;

        protected override void Init(AbilitySystemComponent source)
        {
            base.Init(source);
            _animator = source.GetComponent<Animator>();
        }

        public override void Trigger(AbilitySystemComponent source)
        {
            base.Trigger(source);
            _animator.Play(_animationName);
        }
    }
}